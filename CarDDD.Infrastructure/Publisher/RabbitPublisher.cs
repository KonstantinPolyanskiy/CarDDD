using System.Text.Json;
using CarDDD.Core.AnswerObjects.Result;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace CarDDD.Infrastructure.Publisher;

public class RabbitPublisher(IConnection connection, ILogger<RabbitPublisher> logger) : IIntegrationPublisher
{
    private const string RoutingKeyName = "email";
    private const string ExchangeName = "notifications";
    
    
    public async Task<Result<bool>> PublishAsync<T>(T msg)
    {
        await using var channel = await connection.CreateChannelAsync();

        await channel.BasicPublishAsync(ExchangeName, RoutingKeyName, false,
            CreateBasicPropertiesEmailEvent(), 
            JsonSerializer.SerializeToUtf8Bytes(msg));
        
        await channel.CloseAsync();
        
        logger.LogInformation($"Сообщение {typeof(T).Name} отправлено в rabbit");
        
        return Result<bool>.Success(true);
    }
    
    private BasicProperties CreateBasicPropertiesEmailEvent()
        => new()
        {
            ContentType = "application/json",
            ContentEncoding = "UTF-8",
            MessageId = Guid.NewGuid().ToString(),
            DeliveryMode = DeliveryModes.Persistent,
        };
}