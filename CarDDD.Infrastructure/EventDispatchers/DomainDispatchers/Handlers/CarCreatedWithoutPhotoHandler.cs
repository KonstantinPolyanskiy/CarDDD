using CarDDD.Core.DomainEvents;
using CarDDD.Core.DomainObjects.DomainCar;
using CarDDD.Core.IntegrationEvents;
using CarDDD.Infrastructure.Publisher;

namespace CarDDD.Infrastructure.EventDispatchers.DomainDispatchers.Handlers;

public sealed class CreatedWithoutPhotoHandler(IIntegrationPublisher publisher) : IDomainEventHandler<CreatedWithoutPhoto>
{
    public async Task HandleAsync(CreatedWithoutPhoto evt, CancellationToken ct = default)
    {
        //TODO: получать данные из keycloak
        var msg = new CarCreatedWithoutPhotoMessage(evt.CarId, "manager@mail.com", "Константин Полянский");
            
        await publisher.PublishAsync(msg);
    }
}