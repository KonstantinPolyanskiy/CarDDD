using CarDDD.DomainServices.BaseDomainEvents;
using CarDDD.DomainServices.DomainAggregates.CarAggregate.Events;
using Microsoft.Extensions.Logging;

namespace CarDDD.ApplicationServices.EventHandlers.DomainEventHandlers.CarHandlers;

public class CarUpdatedBasisAttributesHandler(ILogger<CarUpdatedBasisAttributesHandler> log) : IDomainEventHandler<CarUpdatedBasisAttributes>
{
    public Task HandleAsync(CarUpdatedBasisAttributes @event, CancellationToken ct = default)
    {
        log.LogInformation("Manager {manager} update basic car attributes for {car}",
            @event.EmployerId.Value,
            @event.CarId.Value
        );
        
        return Task.CompletedTask;
    }
}

public class CarManagerChangedHandler(ILogger<CarManagerChangedHandler> log) : IDomainEventHandler<CarManagerChanged>
{
    public Task HandleAsync(CarManagerChanged @event, CancellationToken ct = default)
    {
        log.LogInformation("Employee {changer} changed manager {old} to replace {new} at car {car}",
            @event.AdminId.Value,
            @event.OldManagerId.Value,
            @event.NewManagerId.Value,
            @event.CarId.Value
        );
        
        return Task.CompletedTask;
    }
}