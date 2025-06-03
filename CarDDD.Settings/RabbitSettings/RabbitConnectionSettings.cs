namespace CarDDD.Settings.RabbitSettings;

/// <summary>
/// Настройки для подключения к RabbitMQ
/// </summary>
public sealed class RabbitConnectionSettings
{
    /// <summary> Хост запущенного RabbitMQ </summary>
    public string HostName { get; init; } = "localhost";
    
    /// <summary> Пользователь RabbitMQ. </summary>
    public string UserName { get; init; } = "guest";

    /// <summary> Пароль RabbitMQ. </summary>
    public string Password { get; init; } = "guest";
    
    /// <summary> Имя подключения к RabbitMQ </summary>
    public string RabbitClientName { get; init; } = "UnknownRabbitClient";
}