using CarDDD.Core.DomainEvents;

namespace CarDDD.Core.DomainObjects.DomainCar.Events;

public sealed class CreatedWithoutPhoto(Guid CarId, Guid ManagerId) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public sealed class CarSold(Guid CarId, Guid ManagerId, Guid ConsumerId) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}