using System.Reflection;
using CarDDD.Contracts.RabbitContracts;

namespace CarDDD.Infrastructure.Publisher.Rabbit;

/// <summary>
/// Определяет аттрибуты для сообщений в Rabbit (Топик и Routing Key)
/// </summary>
public sealed class RabbitRouteResolver
{
    /// <summary>
    /// Разрешает атрибуты Rabbit для record'ов с <see cref="RabbitRouteAttribute"/>
    /// </summary>
    /// <param name="mt">Тип сообщения для публикации в очередь Rabbit </param>
    public (string exchange, string routingKey) Resolve(Type mt)
    {
        var attr = mt.GetCustomAttribute<RabbitRouteAttribute>()
                   ?? throw new InvalidOperationException(
                       $"RabbitRouteAttribute не найден для сообщения типа: {mt.FullName}");
        
        return (attr.Exchange, attr.RoutingKey);
    }
}