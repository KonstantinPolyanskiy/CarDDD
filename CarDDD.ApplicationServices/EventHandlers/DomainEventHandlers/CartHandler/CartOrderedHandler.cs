using CarDDD.ApplicationServices.Publishers;
using CarDDD.ApplicationServices.Repositories;
using CarDDD.ApplicationServices.Storages;
using CarDDD.Contracts.EmailContracts.EmailNotifications;
using CarDDD.DomainServices.BaseDomainEvents;
using CarDDD.DomainServices.DomainAggregates.CarAggregate;
using CarDDD.DomainServices.DomainAggregates.CartAggregate.Events;
using Microsoft.Extensions.Logging;

namespace CarDDD.ApplicationServices.EventHandlers.DomainEventHandlers.CartHandler;

public class CartOrderedHandler(ILogger<CartOrderedHandler> log, IIntegrationPublisher publisher, 
    IApplicationUserStorage userStorage, ICarRepositoryReader carReader) : IDomainEventHandler<CartOrdered>
{
    public async Task HandleAsync(CartOrdered @event, CancellationToken ct = default)
    {
        log.LogInformation("Customer {customer} order cart {cart}",
            @event.CustomerId.Value,
            @event.CartId.Value
        );

        var orderedCars = new List<Car>();

        foreach (var orderedCar in @event.OrderedCars)
        {
            var car = await carReader.GetAsync(orderedCar, ct);
            if (car != null)
                orderedCars.Add(car);
        }
        
        var carsCount = orderedCars.Count;
        var carsPrice = orderedCars.Sum(x => x.Price);
        
        
        var customer = await userStorage.ReadAsync(@event.CustomerId.Value);
        if (customer is not null)
        {
            var fullName = $"{customer.FirstName} {customer.LastName}";

            await publisher.PublishAsync(
                new CustomerOrderedCartInfoEmailNotification(
                    customer.Email,
                    fullName,
                    carsPrice,
                    carsCount
                )
            );
        }
    }
}