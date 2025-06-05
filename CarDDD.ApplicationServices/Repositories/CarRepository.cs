using CarDDD.ApplicationServices.Models.StorageModels;
using CarDDD.ApplicationServices.Storages;
using CarDDD.DomainServices.DomainAggregates.CarAggregate;
using CarDDD.DomainServices.ValueObjects;

namespace CarDDD.ApplicationServices.Repositories;

public interface ICarRepository
{
    public Task<bool> AddAsync(Car car, CancellationToken ct = default);
    public Task<bool> UpdateAsync(Car car, CancellationToken ct = default);
    
    public Task<Car?> GetOneAsync(CarId carId, CancellationToken ct = default);
}


public class CarRepository(ICarSnapshotReaderWriter snapshots) : ICarRepository
{
    public async Task<bool> AddAsync(Car car, CancellationToken ct = default)
    {
        var carSnapshot = new CarSnapshot
        {
            Id = car.EntityId,
            PhotoId = car.Photo.Id,
            PhotoExtension = car.Photo.Extension,
            ManagerId = car.ManagerId.Value,
            Brand = car.Brand,
            Color = car.Color,
            Price = car.Price,
            Mileage = car.Mileage,
            IsAvailable = car.IsAvailable,
        };
        
        return await snapshots.Writer.WriteAsync(carSnapshot, ct);
    }
    
    // Идейно snapshot - снимок состояния данных, и мы просто перезаписываем уже существующую таблицу
    public async Task<bool> UpdateAsync(Car car, CancellationToken ct = default)
    {
        var snapshot = await snapshots.Reader.ReadOneAsync(car.EntityId, ct);
        if (snapshot is null)
            return false;

        snapshot.PhotoId = car.Photo.Id;
        snapshot.PhotoExtension = car.Photo.Extension;
        snapshot.ManagerId = car.ManagerId.Value;
        snapshot.Brand = car.Brand;
        snapshot.Color = car.Color;
        snapshot.Price = car.Price;
        snapshot.Mileage = car.Mileage;
        snapshot.IsAvailable = car.IsAvailable;
        
        return await snapshots.Writer.WriteAsync(snapshot, ct);
    }

    public async Task<Car?> GetOneAsync(CarId carId, CancellationToken ct = default)
    {
        var snapshot = await snapshots.Reader.ReadOneAsync(carId.Value, ct);
        if (snapshot is null)
            return null;

        return Car.Restore(
            carId,
            snapshot.Brand,
            snapshot.Color,
            snapshot.Price,
            snapshot.Mileage,
            PhotoId.From(snapshot.PhotoId),
            snapshot.PhotoExtension,
            snapshot.IsAvailable,
            EmployerId.From(snapshot.ManagerId)
        );
    }
}