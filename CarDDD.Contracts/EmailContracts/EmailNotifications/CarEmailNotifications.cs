using CarDDD.Contracts.RabbitContracts;
using CarDDD.Settings.RabbitSettings;

namespace CarDDD.Contracts.EmailContracts.EmailNotifications;

/// <summary>
/// Email менеджеру, что машина добавлена без фото 
/// </summary>
[RabbitRoute(RabbitEmail.NotifyExchange, RabbitEmail.RoutingPrefix + ".employer")]
public record ManagerCreatedCarWithoutPhotoNotification(Guid CarId, string ManagerEmail, string ManagerFullName) : IEmailNotification
{
    public string To() => ManagerEmail;
}


