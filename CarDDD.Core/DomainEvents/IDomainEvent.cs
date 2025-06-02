namespace CarDDD.Core.DomainEvents;

public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}