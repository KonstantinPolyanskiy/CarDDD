using CarDDD.ApplicationServices.Publishers;
using CarDDD.ApplicationServices.Storages;
using CarDDD.Contracts.EmailContracts.EmailNotifications;
using CarDDD.DomainServices.BaseDomainEvents;
using CarDDD.DomainServices.DomainAggregates.CarAggregate.Events;
using Microsoft.Extensions.Logging;

namespace CarDDD.ApplicationServices.EventHandlers.DomainEventHandlers.CarHandlers;

public class CarCreatedHandler(ILogger<CarCreatedHandler> log, IIntegrationPublisher publisher, IApplicationUserReader userReader) : IDomainEventHandler<CarCreated>
{
    public async Task HandleAsync(CarCreated @event, CancellationToken ct = default)
    {
        log.LogInformation("Domain car {CarId} created by user {UserId}, with photo - {HasPhoto}",
            @event.CarId.Value,
            @event.ManagerId.Value,
            @event.HasPhoto);
        
        // Нет фото - шлем уведомление
        if (!@event.HasPhoto)
        {
            var manager = await userReader.ReadOnceAsync(@event.ManagerId.Value);
            if (manager == null)
            {
                log.LogWarning("Cant get data for manager {guid}", @event.ManagerId.Value);
                return;
            }

            var fullName = $"{manager.FirstName} {manager.LastName}";

            await publisher.PublishAsync(
                new ManagerCreatedCarWithoutPhotoNotification(
                    @event.CarId.Value,
                    manager.Email,
                    fullName
                )
            );
        }
    }
}