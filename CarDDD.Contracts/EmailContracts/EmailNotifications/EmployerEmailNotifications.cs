using CarDDD.Contracts.RabbitContracts;
using CarDDD.Settings.RabbitSettings;

namespace CarDDD.Contracts.EmailContracts.EmailNotifications;

/// <summary>
/// Письмо администратору о заказанных клиентом машинах 
/// </summary>
[RabbitRoute(RabbitEmail.NotifyExchange, RabbitEmail.RoutingPrefix + ".employer")]
public record EmployerOrderedCartInfoEmailNotification(string AdminEmail, string PurchaserFullName, decimal TotalPrice, decimal TotalCount, IReadOnlyList<ManagerOrderedCars> OrderedCars)
    : IEmailNotification
{
    public string To() => AdminEmail;
}

/// <summary>
/// Менеджер со списком машин, заказанных клиентом  
/// </summary>
public record ManagerOrderedCars
{
    public ManagerOrderedCars(Guid managerId, List<Guid> carIds)
    {
        ManagerId = managerId;
        CarIds = carIds;
    }
    
    public Guid ManagerId { get; set; }
    public IReadOnlyList<Guid> CarIds { get; set; } = new List<Guid>();
}