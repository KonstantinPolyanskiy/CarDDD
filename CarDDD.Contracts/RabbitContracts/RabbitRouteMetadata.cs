namespace CarDDD.Contracts.RabbitContracts;

/// <summary>
/// Атрибут с топиком и routing key для определения куда слать сообщения
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class RabbitRouteAttribute(string exchange, string routingKey) : Attribute
{
    public string Exchange   { get; } = exchange;
    public string RoutingKey { get; } = routingKey;
}