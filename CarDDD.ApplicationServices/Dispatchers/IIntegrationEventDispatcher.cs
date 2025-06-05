using CarDDD.ApplicationServices.Models.Events;

namespace CarDDD.ApplicationServices.Dispatchers;

public interface IIntegrationEventDispatcher
{
    public Task DispatchAsync(IEnumerable<IIntegrationEvent> events, CancellationToken ct = default);
}