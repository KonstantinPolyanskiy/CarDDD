using Microsoft.Extensions.DependencyInjection;

namespace CarDDD.Infrastructure.EventDispatchers.DomainDispatchers;

public sealed class DomainEventDispatcher(IServiceProvider sp) : IDomainEventDispatcher
{
    public async Task DispatchAsync(
        IEnumerable<CarDDD.Core.DomainEvents.IDomainEvent> events,
        CancellationToken ct = default)
    {
        foreach (var evt in events)
        {
            var handlerType = typeof(CarDDD.Core.DomainEvents.IDomainEventHandler<>)
                .MakeGenericType(evt.GetType());

            foreach (var handler in sp.GetServices(handlerType))
            {
                await (Task)handlerType
                    .GetMethod("HandleAsync")!
                    .Invoke(handler, new object?[] { evt, ct })!;
            }
        }
    }
}