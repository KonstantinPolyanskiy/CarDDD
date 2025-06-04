namespace CarDDD.DomainServices.BaseDomainEvents;

public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}