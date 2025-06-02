namespace CarDDD.Core.DomainEvents;

public sealed record CarCreated(Guid CarId) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public sealed record CreatedWithoutPhoto(Guid CarId) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}