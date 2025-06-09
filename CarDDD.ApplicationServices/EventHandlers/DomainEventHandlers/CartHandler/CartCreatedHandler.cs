using CarDDD.DomainServices.BaseDomainEvents;
using CarDDD.DomainServices.DomainAggregates.CartAggregate.Events;
using Microsoft.Extensions.Logging;

namespace CarDDD.ApplicationServices.EventHandlers.DomainEventHandlers.CartHandler;

public class CartCreatedHandler(ILogger<CartCreatedHandler> log) : IDomainEventHandler<CartCreated>
{
    public Task HandleAsync(CartCreated @event, CancellationToken ct = default)
    {
        log.LogInformation("For customer {customer} created cart {cart}",
            @event.CustomerId.Value,
            @event.CartId.Value
        );
        
        return Task.CompletedTask;
    }
}