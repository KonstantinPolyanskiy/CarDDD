using CarDDD.ApplicationServices.Storages;
using CarDDD.DomainServices.BaseDomainEvents;
using CarDDD.DomainServices.DomainAggregates.CarAggregate.Events;
using Microsoft.Extensions.Logging;

namespace CarDDD.ApplicationServices.EventHandlers.DomainEventHandlers.CarHandlers;

public class CarPhotoAttachedHandler(IPhotoReader tempReader, IPhotoWriter mainWriter, ILogger<CarPhotoAttachedHandler> log) : IDomainEventHandler<CarPhotoAttached>
{
    public async Task HandleAsync(CarPhotoAttached @event, CancellationToken ct = default)
    {
        var saved = false;
        var photoData = await tempReader.ReadOneAsync(@event.PhotoId.Value, ct);
        if (photoData != null)
            saved = await mainWriter.WriteAsync(photoData, ct);
        
        log.LogWarning("Temporary photo reader not found attached photo data"); // В будущем можно создать event о потере фото и т.д.

        log.LogInformation("Photo {photo} attached to car {car}, saved - {saved}",
            @event.PhotoId.Value,
            @event.CarId.Value,
            saved
        );
    }
}