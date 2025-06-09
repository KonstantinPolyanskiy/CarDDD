using CarDDD.DomainServices.BaseDomainEvents;
using CarDDD.DomainServices.DomainAggregates.CartAggregate.Events;
using Microsoft.Extensions.Logging;

namespace CarDDD.ApplicationServices.EventHandlers.DomainEventHandlers.CartHandler;

public class AddedCarInCartHandler(ILogger<AddedCarInCartHandler> log) : IDomainEventHandler<AddedCarInCart>
{
    public Task HandleAsync(AddedCarInCart @event, CancellationToken ct = default)
    {
        log.LogInformation("Customer {customer} add car {car} in cart {cart}",
            @event.CustomerId.Value,
            @event.CarId.Value,
            @event.CartId.Value
        );
        
        return Task.CompletedTask;
    }
}