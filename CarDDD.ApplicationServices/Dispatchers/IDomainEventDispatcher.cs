using CarDDD.DomainServices.BaseDomainEvents;

namespace CarDDD.ApplicationServices.Dispatchers;

public interface IDomainEventDispatcher
{
    public Task DispatchAsync(IEnumerable<IDomainEvent> events, CancellationToken ct = default);
}

public class SimpleDomainEventDispatcher(IServiceProvider sp) : IDomainEventDispatcher
{
    public async Task DispatchAsync(IEnumerable<IDomainEvent> events, CancellationToken ct = default)
    {
        foreach (var @event in events)
        {
            var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(@event.GetType());

            var handlers = (IEnumerable<object>) sp
                .GetService(typeof(IEnumerable<>).MakeGenericType(handlerType))!;

            foreach (var handler in handlers)
            {
                var handleMethod = handlerType.GetMethod("HandleAsync")!;
                await (Task) handleMethod.Invoke(handler, [@event, ct])!;
            }
        }
    }
}