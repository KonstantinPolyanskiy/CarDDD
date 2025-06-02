using CarDDD.Core.DomainObjects.CommonValueObjects;
using CarDDD.Core.DomainObjects.DomainCar;
using CarDDD.Core.EntityObjects;
using MediatR;

namespace CarDDD.Core.IntegrationEvents;

public record CarCreatedWithoutPhotoMessage(Guid CarId, string ManagerEmail, string ManagerFullName);

public record ConsumerOrderedCartInfoMessage(string PurchaserEmail, string PurchaserFullName, decimal TotalPrice, decimal TotalCount);

public record EmployerOrderedCartInfoMessage(string AdminEmail, string PurchaserFullName, decimal TotalPrice, decimal TotalCount, IReadOnlyList<ManagerOrderedCars> OrderedCars);

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