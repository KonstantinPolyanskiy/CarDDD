namespace CarDDD.Core.IntegrationEvents;

public class EmailNotificationEvent
{
    public required Guid MessageId { get; init; } = Guid.NewGuid();
    
    public required string To { get; init; }
    public required string Subject { get; init; }
    
    public required string BodyHtml { get; init; }
}