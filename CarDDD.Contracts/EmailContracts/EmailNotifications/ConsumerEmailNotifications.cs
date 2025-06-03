using CarDDD.Contracts.RabbitContracts;
using CarDDD.Settings.RabbitSettings;

namespace CarDDD.Contracts.EmailContracts.EmailNotifications;

/// <summary>
/// Email на почту клиента о заказе корзины
/// </summary>
[RabbitRoute(RabbitEmail.NotifyExchange, RabbitEmail.RoutingPrefix + ".customer")]
public record ConsumerOrderedCartInfoEmailNotification(string PurchaserEmail, string CustomerFullName, decimal TotalPrice, decimal TotalCount) : IEmailNotification
{
    public string To() => PurchaserEmail;
}
