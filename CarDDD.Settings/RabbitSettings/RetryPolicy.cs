namespace CarDDD.Settings.RabbitSettings;

/// <summary> Настройки повторов для очередей RabbitMq </summary>
public class RabbitRetryPolicy
{
    /// <summary>
    /// Кол-во повторов
    /// </summary>
    public byte Count { get; set; } = 5;
    
    /// <summary>
    /// Задержка в мс между повторами
    /// </summary>
    public uint Timeout { get; set; } = 1000;
}