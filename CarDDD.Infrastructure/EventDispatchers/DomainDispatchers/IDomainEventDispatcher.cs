namespace CarDDD.Infrastructure.EventDispatchers.DomainDispatchers;

public interface IDomainEventDispatcher
{
    public Task DispatchAsync(IEnumerable<Core.DomainEvents.IDomainEvent> events, CancellationToken ct = default);
}