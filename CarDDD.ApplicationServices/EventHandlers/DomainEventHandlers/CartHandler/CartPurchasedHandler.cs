using CarDDD.ApplicationServices.Models.RequestModels;
using CarDDD.ApplicationServices.Publishers;
using CarDDD.ApplicationServices.Repositories;
using CarDDD.ApplicationServices.Services;
using CarDDD.DomainServices.BaseDomainEvents;
using CarDDD.DomainServices.DomainAggregates.CarAggregate.Results;
using CarDDD.DomainServices.DomainAggregates.CartAggregate.Events;
using Microsoft.Extensions.Logging;

namespace CarDDD.ApplicationServices.EventHandlers.DomainEventHandlers.CartHandler;

public class CartPurchasedHandler(ILogger<CartPurchasedHandler> log, ICarMutableService seller) 
    : IDomainEventHandler<CartPurchased> 
{
    public async Task HandleAsync(CartPurchased @event, CancellationToken ct = default)
    {
        log.LogInformation("Customer {customer} purchased cart {cart}",
            @event.CustomerId.Value,
            @event.CartId.Value
        );

        foreach (var carId in @event.PurchasedCars)
        {
            var sold = await seller.SellCarAsync(new SellCarRequest
            {
                CarId = carId.Value,
                CustomerId = @event.CustomerId.Value,
            }, ct);

            if (sold.IsFailure)
                log.LogWarning("Error sold car, error - {@error}", sold.Error);
            else
                log.LogInformation("Successfully sold car {carId}", carId.Value);
        }
    }
}