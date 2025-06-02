
using CarDDD.Core.DomainObjects.DomainCar;
using CarDDD.Core.SnapshotModels;

namespace CarDDD.Infrastructure.Repositories;

public interface ICarRepository
{
    public Task<bool> AddCarAsync(Car car);
    public Task<bool> UpdateCarAsync(Car car);
    
    public Task<Car?> FindByIdAsync(Guid carId);
}