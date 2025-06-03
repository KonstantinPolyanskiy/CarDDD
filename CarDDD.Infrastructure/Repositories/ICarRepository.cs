
using CarDDD.Core.AnswerObjects.Result;
using CarDDD.Core.DomainObjects.DomainCar;
using CarDDD.Core.DtoObjects;

namespace CarDDD.Infrastructure.Repositories;

public interface ICarRepository
{
    public Task<bool> AddCarAsync(Car car);
    public Task<bool> UpdateCarAsync(Car car);
    
    public Task<Car?> FindByIdAsync(Guid carId);
    public Task<PageResult<Car>?> FindByParams(SearchCarDto dto);
}