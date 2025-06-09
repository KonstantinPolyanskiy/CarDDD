using System.Text.Json;
using System.Text;
using CarDDD.ApplicationServices.Models.AnswerObjects.Result;
using CarDDD.ApplicationServices.Publishers;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace CarDDD.Infrastructure.Publisher.Rabbit;

/// <summary>
/// Публикует сообщения в брокер Rabbit 
/// </summary>
public class RabbitPublisher(IConnection connection, RabbitRouteResolver resolver, ILogger<RabbitPublisher> log) : IIntegrationPublisher
{
    public async Task<Result<bool>> PublishAsync<T>(T msg)
    {
        if (msg == null)
        {
            log.LogWarning("Сообщение для публикации в Rabbit было null");
            return Result<bool>.Failure(Error.Infrastructure(ErrorType.NotFound, "Сообщения для публикации в Rabbit было null"));
        }

        var (exchange, key) = resolver.Resolve(msg.GetType());
        
        await using var channel = await connection.CreateChannelAsync();

        var props = CreateBasicProperties();
        
        var typeName = typeof(T).FullName;
        if (typeName == null)
        {
            log.LogWarning("Не удалось получить название типа для {msg}", msg);
            return Result<bool>.Failure(Error.Infrastructure(ErrorType.NotFound, "Название типа переданного сообщения было null"));
        }
        
        SetMessageTypeHeaders(props, typeName);

        await channel.BasicPublishAsync(exchange, key, false, props,
            JsonSerializer.SerializeToUtf8Bytes(msg));
        
        await channel.CloseAsync();
        
        log.LogInformation("Сообщение с типом {typeName} отправлено в rabbit, тело {@body}", typeName, msg);
        
        return Result<bool>.Success(true);
    }
    
    /// <summary>
    /// Создает базовые свойства публикации сообщения.
    /// С типом json, кодировкой utf-8, сгенерированным guid и сохранением на диске
    /// </summary>
    private BasicProperties CreateBasicProperties()
        => new()
        {
            ContentType = "application/json",
            ContentEncoding = "UTF-8",
            MessageId = Guid.NewGuid().ToString(),
            DeliveryMode = DeliveryModes.Persistent,
        };

    /// <summary>
    /// Устанавливает заголовок Type (тип структуры-сообщения) для <see cref="BasicProperties"/> с utf-8 кодировкой
    /// </summary>
    private static void SetMessageTypeHeaders(BasicProperties props, string fullTypeName)
    {
        props.Headers ??= new Dictionary<string, object>()!;
        
        props.Headers["type"] = Encoding.UTF8.GetBytes(fullTypeName);
    }
}