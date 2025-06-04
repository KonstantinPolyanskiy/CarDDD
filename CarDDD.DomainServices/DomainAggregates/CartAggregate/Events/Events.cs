using CarDDD.DomainServices.BaseDomainEvents;
using CarDDD.DomainServices.ValueObjects;

namespace CarDDD.DomainServices.DomainAggregates.CartAggregate.Events;

public sealed record CartCreated(CartId CartId, CustomerId CustomerId) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public sealed record CartOrdered(CartId CartId, IReadOnlyList<CarId> OrderedCars, CustomerId CustomerId) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public sealed record CartPurchased(CartId CartId, IReadOnlyList<CarId> PurchasedCars, CustomerId CustomerId) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public sealed record AddedCarInCart(CartId CartId, CustomerId CustomerId, CarId CarId) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public sealed record RemovedCarFromCart(CartId CartId, CustomerId CustomerId, CarId CarId) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}


