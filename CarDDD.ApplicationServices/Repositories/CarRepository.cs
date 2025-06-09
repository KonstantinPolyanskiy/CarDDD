using CarDDD.ApplicationServices.Models.StorageModels;
using CarDDD.ApplicationServices.Storages;
using CarDDD.DomainServices.DomainAggregates.CarAggregate;
using CarDDD.DomainServices.ValueObjects;

namespace CarDDD.ApplicationServices.Repositories;

/// <summary>
/// Интерфейс для чтения domain модели Машина
/// </summary>
public interface ICarRepositoryReader
{
    public Task<Car?> GetAsync(CarId carId, CancellationToken ct = default);
    public Task<IQueryable<Car>> CarsQueryAsync();    
}

public interface ICarRepository
{
    public Task<bool> AddAsync(Car car, CancellationToken ct = default);
    public Task<bool> UpdateAsync(Car car, CancellationToken ct = default);
    
    public Task<Car?> GetAsync(CarId carId, CancellationToken ct = default);
}


public class CarRepository(ICarSnapshotStorage snapshots) : ICarRepository, ICarRepositoryReader
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
        
        return await snapshots.SaveAsync(carSnapshot, ct);
    }
    
    // Идейно snapshot - снимок состояния данных, и мы просто перезаписываем уже существующую таблицу
    public async Task<bool> UpdateAsync(Car car, CancellationToken ct = default)
    {
        var snapshot = await snapshots.ReadAsync(car.EntityId, ct);
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
        
        return await snapshots.SaveAsync(snapshot, ct);
    }

    public async Task<bool> UpdateAsync(IEnumerable<Car> cars, CancellationToken ct = default)
    {
        var originalSnapshots = new List<CarSnapshot>();

        foreach (var car in cars.ToList())
        {
            // Находим снимок для машины
            var snapshot = await snapshots.ReadAsync(car.EntityId, ct);
            if (snapshot is null) 
            {
                foreach (var original in originalSnapshots)
                    await snapshots.SaveAsync(original, ct); // Если снимок не найден - откатываем все ранее обновленные машины 
                
                return false;
            }
            
            // Добавляем найденный снимок в список изначальных
            originalSnapshots.Add(new CarSnapshot
            {
                Id = snapshot.Id,
                PhotoId = snapshot.PhotoId,
                PhotoExtension = snapshot.PhotoExtension,
                ManagerId = snapshot.ManagerId,
                Brand = snapshot.Brand,
                Color = snapshot.Color,
                Price = snapshot.Price,
                Mileage = snapshot.Mileage,
                IsAvailable = snapshot.IsAvailable,
            });
            
            // Создаем снимок с изменениями для машины
            var updatedSnapshot = new CarSnapshot
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
            
            // Перезаписываем данные со снимком с изменениями
            var updated = await snapshots.SaveAsync(updatedSnapshot, ct);
            if (!updated) // Если не получилось - откатываем все обратно
            {
                foreach (var original in originalSnapshots)
                {
                    await snapshots.SaveAsync(original, ct);
                }
                
                return false;
            }
        }
        
        return true;
    }

    public async Task<Car?> GetAsync(CarId carId, CancellationToken ct = default)
    {
        var snapshot = await snapshots.ReadAsync(carId.Value, ct);
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

    public async Task<IQueryable<Car>> CarsQueryAsync()
    {
        var snapshotsQuery = await snapshots.QueryForReadAllAsync();
        
        return snapshotsQuery.Select(snapshot => Car.Restore(
            CarId.From(snapshot.Id),
            snapshot.Brand,
            snapshot.Color,
            snapshot.Price,
            snapshot.Mileage,
            PhotoId.From(snapshot.PhotoId),
            snapshot.PhotoExtension,
            snapshot.IsAvailable,
            EmployerId.From(snapshot.ManagerId)
        ));
    }
}