using CarDDD.DomainServices.BaseDomainEvents;
using CarDDD.DomainServices.DomainAggregates.CarAggregate.Events;
using Microsoft.Extensions.Logging;

namespace CarDDD.ApplicationServices.EventHandlers.DomainEventHandlers.CarHandlers;

public class CarSoldHandler(ILogger<CarSoldHandler> log) : IDomainEventHandler<CarSold>
{
    public Task HandleAsync(CarSold @event, CancellationToken ct = default)
    {
        log.LogInformation("Машина {carId} менеджера {managerId} была куплена клиентом {customerId}",
            @event.CarId.Value,
            @event.ManagerId.Value,
            @event.CustomerId.Value
        );
        
        return Task.CompletedTask;
    }
}
