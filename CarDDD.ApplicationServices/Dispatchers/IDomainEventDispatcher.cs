using CarDDD.DomainServices.BaseDomainEvents;

namespace CarDDD.ApplicationServices.Dispatchers;

public interface IDomainEventDispatcher
{
    public Task DispatchAsync(IEnumerable<IDomainEvent> events, CancellationToken ct = default);
}