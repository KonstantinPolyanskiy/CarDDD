using CarDDD.DomainServices.BaseDomainEvents;
using CarDDD.DomainServices.DomainAggregates.CartAggregate.Events;
using Microsoft.Extensions.Logging;

namespace CarDDD.ApplicationServices.EventHandlers.DomainEventHandlers.CartHandler;

public class RemovedCarFromCartHandler(ILogger<RemovedCarFromCartHandler> log) : IDomainEventHandler<RemovedCarFromCart>
{
    public Task HandleAsync(RemovedCarFromCart @event, CancellationToken ct = default)
    {
        log.LogInformation("Customer {customer} remove car {car} from cart {cart}",
            @event.CustomerId.Value,
            @event.CarId.Value,
            @event.CartId.Value
        );
        
        return Task.CompletedTask;
    }
}