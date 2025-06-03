namespace CarDDD.Settings.RabbitSettings;

/// <summary>
/// Данные для отправки/чтения почтовых уведомлений
/// </summary>
public sealed class RabbitEmail
{
    public const string NotifyExchange = "notifications";
    
    public const string RoutingPrefix = "email";         
    public const string Queue = "email-notify";

    public const string DeadExchange = NotifyExchange + "-dead";
    public const string DeadQueue = Queue    + "-dead";
    public const string RetryExchange = NotifyExchange + "-retry";
}