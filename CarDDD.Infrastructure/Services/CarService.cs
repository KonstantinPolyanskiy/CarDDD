using System.Security.Claims;
using CarDDD.Core.AnswerObjects.Result;
using CarDDD.Core.AnswerObjects.ServiceResponses;
using CarDDD.Core.DomainEvents;
using CarDDD.Core.DomainObjects;
using CarDDD.Core.DomainObjects.DomainCar;
using CarDDD.Core.DomainObjects.DomainCar.Actions;
using CarDDD.Core.DtoObjects;
using CarDDD.Infrastructure.EventDispatchers.DomainDispatchers;
using CarDDD.Infrastructure.Repositories;

namespace CarDDD.Infrastructure.Services;

public class CarService(ICarRepository cars, IDomainEventDispatcher dispatcher)
{
    public async Task<Result<CarInfo>> CreateAsync(AddNewCarDto dto, ClaimsPrincipal user)
    {
        var previousOwner = dto.PreviousOwner is null ? 
            Ownership.None : 
            Ownership.PreviousOwnership(dto.PreviousOwner.Name, dto.PreviousOwner.Mileage);
        
        var photo  = dto.Photo is null ? 
            Photo.None :
            Photo.From(dto.Photo.Extension, dto.Photo.Data);

        var carResult = Car.Create(
            dto.Brand,
            dto.Color,
            dto.Price,
            previousOwner,
            photo,
            Employer.From(Guid.NewGuid(), [Role.CarAdmin, Role.CarConsumer, Role.CarManager]) //TODO: заменить на claims пользователя
        );
        
        if (carResult.Status is not CreateCarAction.Success)
            return Result<CarInfo>.Failure(Error.Domain(ErrorType.Conflict, carResult.Status.ToString()));
        
        var car = carResult.Car;
        if (car is null)
            return Result<CarInfo>.Failure(Error.Domain(ErrorType.Unknown, "No car"));

        var saved = await cars.AddCarAsync(car);
        if (!saved)
            return Result<CarInfo>.Failure(Error.Application(ErrorType.Unknown, "Car not saved"));

        await dispatcher.DispatchAsync(car.DomainEvents);
        car.ClearAllDomainEvents();
        
        return Result<CarInfo>.Success(new CarInfo(car));
    }

    public async Task<Result<CarInfo>> UpdateAsync(Guid carId, UpdateCarDto dto, ClaimsPrincipal user)
    {
        var car = await cars.FindByIdAsync(carId);
        if (car is null)
            return Result<CarInfo>.Failure(Error.Application(ErrorType.NotFound, "No car"));

        car.Update(
            dto.Brand,
            dto.Color,
            dto.Price,
            new Employer { EntityId = Guid.NewGuid() }
        );

        var updated = await cars.UpdateCarAsync(car);
        if (!updated)
            return Result<CarInfo>.Failure(Error.Application(ErrorType.Unknown, "Car not updated"));
        
        await dispatcher.DispatchAsync(car.DomainEvents);
        car.ClearAllDomainEvents();
        
        return Result<CarInfo>.Success(new CarInfo(car)); 
    }

    public async Task<Result<CarInfo>> GetByIdAsync(Guid carId, ClaimsPrincipal user)
    {
        var car = await cars.FindByIdAsync(carId);
        if (car is null)
            return Result<CarInfo>.Failure(Error.Application(ErrorType.NotFound, "No car"));
        
        return Result<CarInfo>.Success(new CarInfo(car));
    }

    public async Task<Result<PageResult<CarInfo>>> GetByParamsAsync(SearchCarDto dto, ClaimsPrincipal user)
    {
        var carPage = await cars.FindByParams(dto);
        if (carPage is null)
            return Result<PageResult<CarInfo>>.Failure(Error.Application(ErrorType.NotFound, "No car"));
        
        var carInfos = carPage.Items.Select(c => new CarInfo(c)).ToList();
        var resp = await PageResult<CarInfo>.CreateAsync(carInfos.AsQueryable(), dto.PageSize, dto.PageNumber);
                    
        return Result<PageResult<CarInfo>>.Success(resp);
    }
}