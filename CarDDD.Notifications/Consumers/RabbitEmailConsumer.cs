using System.Text;
using System.Text.Json;
using CarDDD.Contracts.EmailContracts.EmailNotifications;
using CarDDD.Notifications.Services;
using CarDDD.Settings.RabbitSettings;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CarDDD.Notifications.Consumers;

/// <summary>
/// Читает почтовые уведомления из очереди Rabbit, готовит письма и отправляет их   
/// </summary>
public class RabbitEmailConsumer(IConnection conn, IEmailTemplateService mailTemplate, IMailSender mailSender,
    ILogger<RabbitEmailConsumer> log) : BackgroundService
{
    // Канал для чтения
    private IChannel? _channel; 
    
    // Политика повторных отправок
    private RabbitRetryPolicy RetryPolicy { get; } = new();

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        _channel = await conn.CreateChannelAsync(cancellationToken: ct);
        if (!_channel.IsOpen || _channel == null)
        {
            log.LogWarning("Не удалось создать канал rabbit");
            return;
        }
        
        // Топик основных
        await _channel.ExchangeDeclareAsync(
            exchange: RabbitEmail.NotifyExchange,
            type: ExchangeType.Topic,
            durable: true,
            cancellationToken: ct);
        
        // Топик мертвых (неотправленных) 
        await _channel.ExchangeDeclareAsync(
            exchange: RabbitEmail.DeadExchange,
            type: ExchangeType.Direct,
            durable: true,
            cancellationToken: ct);
        
        // Топик повторной отправвки (retry <- dead)
        await _channel.ExchangeDeclareAsync(
            exchange: RabbitEmail.RetryExchange,
            type: ExchangeType.Direct,
            durable: true,
            cancellationToken: ct);
        
        log.LogInformation("Все топики объявлены успешно");
        
        // Очередь для основных
        await _channel.QueueDeclareAsync(
            queue: RabbitEmail.Queue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: new Dictionary<string, object?>
            {
                { "x-dead-letter-exchange", RabbitEmail.DeadExchange },
                { "x-dead-letter-routing-key", RabbitEmail.DeadQueue }  
            },
            cancellationToken: ct);
        
        // Очередь для мертвых
        await _channel.QueueDeclareAsync(
            queue: RabbitEmail.DeadQueue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: new Dictionary<string, object?>
            {
                { "x-message-ttl", RetryPolicy.Timeout },       
                { "x-dead-letter-exchange", RabbitEmail.RetryExchange } 
            },
            cancellationToken: ct);
        
        log.LogInformation("Все очереди объявлены успешно");
        
        await _channel.QueueBindAsync(
            queue: RabbitEmail.Queue,
            exchange: RabbitEmail.NotifyExchange,
            routingKey: $"{RabbitEmail.RoutingPrefix}.#",
            cancellationToken: ct);

        await _channel.QueueBindAsync(
            queue: RabbitEmail.DeadQueue,
            exchange: RabbitEmail.DeadExchange,
            routingKey: RabbitEmail.DeadQueue,
            cancellationToken: ct);

        await _channel.QueueBindAsync(
            queue: RabbitEmail.Queue,
            exchange: RabbitEmail.RetryExchange,
            routingKey: $"{RabbitEmail.RoutingPrefix}.#",
            cancellationToken: ct);
        
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += OnMessageAsync;

        await _channel.BasicConsumeAsync(
            queue: RabbitEmail.Queue,
            autoAck: false, 
            consumer: consumer,
            cancellationToken: ct);
    }
    
    private async Task OnMessageAsync(object sender, BasicDeliverEventArgs ea)
    {
        try
        {
            var notification = Deserialize(ea, log);
            if (notification == null)
            {
                log.LogWarning("Десериализованное email оповещение было null");
                await NegativeAck(ea);
                
                return;
            }

            var emailMessage = mailTemplate.Create(notification);
            if (emailMessage == null)
            {
                log.LogWarning("Не удалось создать шаблон письма из {@notification}", notification);
                
                return;
            }
            
            await mailSender.SendAsync(emailMessage);
            
            await PositiveAck(ea);
        }
        catch (Exception ex)
        {
            var reject = ExceededRetry(ea);
            log.LogWarning("Ошибка в отправке, отмена доставки: {reject}", reject);
            
            // Проверяем - доступны ли еще попытки
            if (reject)
                await BasicReject(ea);
            else 
                await NegativeAck(ea);
        }
    }
    
    /// <summary>
    /// Десериализует сообщение из Rabbit в <see cref="IEmailNotification"/>
    /// null если неудача
    /// </summary>
    private static IEmailNotification? Deserialize(BasicDeliverEventArgs ea, ILogger log)
    {
        try
        {
            var json = Encoding.UTF8.GetString(ea.Body.Span);

            if (ea.BasicProperties.Headers == null)
            {
                log.LogWarning("Заголовки свойств сообщения были null");
                return null;
            }
            
            var typeNameBytes = ea.BasicProperties.Headers["type"] as byte[];
            if (typeNameBytes == null)
            {
                log.LogWarning("Тип сообщения в сыром виде был null");
                return null;
            }
            
            var typeName = Encoding.UTF8.GetString(typeNameBytes);
            var type = Type.GetType(typeName)!;
            
            return (IEmailNotification)JsonSerializer.Deserialize(json, type)!;
        }
        catch (Exception ex)
        {
            log.LogWarning(ex, ex.Message);
            return null;
        }
    }
    
    /// <summary>
    /// Превышено ли кол-во повторных попыток, если да - true
    /// </summary>
    private bool ExceededRetry(BasicDeliverEventArgs ea)
    {
        if (!ea.BasicProperties.Headers?.TryGetValue("x-death", out var raw) ?? true) return false;
        var death = (IReadOnlyDictionary<string, object>) ((List<object>)raw!)[0];
        var count = (long) death["count"];
        return count >= RetryPolicy.Count;
    }  

    #region Private

    private async Task PositiveAck(BasicDeliverEventArgs ea)
    {
        if (_channel is { IsOpen: true })
            await _channel.BasicAckAsync(ea.DeliveryTag, false);
    }

    private async Task NegativeAck(BasicDeliverEventArgs ea)
    {
        if (_channel is { IsOpen: true })
            await _channel.BasicNackAsync(ea.DeliveryTag, false, false);
    }

    private async Task BasicReject(BasicDeliverEventArgs ea, CancellationToken ct = default)
    {
        if (_channel is { IsOpen: true })
            await _channel.BasicRejectAsync(ea.DeliveryTag, false, ct);
    }
    
    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_channel != null)
            await _channel.CloseAsync(cancellationToken: cancellationToken);
        
        await base.StopAsync(cancellationToken);
    }

    #endregion
    
}