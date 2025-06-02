using CarDDD.Core.DomainEvents;
using CarDDD.Core.DomainObjects.CommonValueObjects;
using CarDDD.Core.DomainObjects.DomainCar.EntityObjects;
using CarDDD.Core.EntityObjects;

namespace CarDDD.Core.DomainObjects.DomainCar.Events;

public sealed class CreatedWithoutPhoto(CarId carId, Manager Manager) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public sealed class CarSold(CarId carId, Manager ManagerId, Consumer Consumer) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public sealed class CarManagerChanged(CarId carId, Manager NewManager, Manager OldManager) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}