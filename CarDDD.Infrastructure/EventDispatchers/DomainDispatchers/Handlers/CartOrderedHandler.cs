using CarDDD.Contracts.EmailContracts.EmailNotifications;
using CarDDD.Core.DomainEvents;
using CarDDD.Core.DomainObjects.DomainCart.Events;
using CarDDD.Infrastructure.Models.SnapshotModels;
using CarDDD.Infrastructure.Publisher;
using CarDDD.Infrastructure.Storages;

namespace CarDDD.Infrastructure.EventDispatchers.DomainDispatchers.Handlers;

public sealed class CartOrderedHandler(IIntegrationPublisher publisher, ICarStorage cars) : IDomainEventHandler<CartOrdered>
{
    public async Task HandleAsync(CartOrdered evt, CancellationToken ct = default)
    {
        var carSnaps = await Task.WhenAll(
            evt.CarIds.Select(id => cars.GetCarSnapshotAsync(id.Value)));
        if (carSnaps.Length == 0) return;
        
        var price = carSnaps.Sum(c => c.Price);
        var count = carSnaps.Length;
        
        await publisher.PublishAsync(CreateConsumerMessage(price, count));
        await publisher.PublishAsync(CreateEmployerMessage(carSnaps!, price, count));
    }

    private ConsumerOrderedCartInfoEmailNotification CreateConsumerMessage(decimal price, int count)
    {
        return new ConsumerOrderedCartInfoEmailNotification(
        "purchaser@mail.com",
        "Кто ктотович",
        price,
        count);
    }

    private EmployerOrderedCartInfoEmailNotification CreateEmployerMessage(CarSnapshot[] carSnapshots, decimal price, int count)
    {

        var byManager = carSnapshots
            .GroupBy(s => s.ManagerId)
            .Select(g => new ManagerOrderedCars(g.Key, g.Select(x => x.Id).ToList()))
            .ToList();

        return new EmployerOrderedCartInfoEmailNotification(
           "admin@mail.com",
           "кто то ктотович",
           price,
           count,
           byManager);
    }
}
