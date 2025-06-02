using CarDDD.Core.DomainEvents;
using CarDDD.Core.DomainObjects.CommonValueObjects;
using CarDDD.Core.DomainObjects.DomainCar;
using CarDDD.Core.EntityObjects;

namespace CarDDD.Core.DomainObjects.DomainCart.Events;

public sealed record CartOrdered(IReadOnlyList<CarId> CarIds, ConsumerId PurchaserId) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}