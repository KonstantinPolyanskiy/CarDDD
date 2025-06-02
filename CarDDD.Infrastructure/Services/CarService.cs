using System.Security.Claims;
using CarDDD.Core.AnswerObjects.Result;
using CarDDD.Core.DomainObjects;
using CarDDD.Core.DomainObjects.DomainCar;
using CarDDD.Core.DomainObjects.DomainCar.Actions;
using CarDDD.Core.DtoObjects;
using CarDDD.Infrastructure.Repositories;

namespace CarDDD.Infrastructure.Services;

public class CarService(ICarRepository carRepository)
{
    public async Task<Result<bool>> CreateAsync(AddNewCarDto dto, ClaimsPrincipal user)
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
            Employer.From(Guid.NewGuid(), [Role.CarAdmin, Role.CarConsumer, Role.CarManager])
        );
        
        if (carResult.Status is not CreateCarAction.Success)
            return Result<bool>.Failure(Error.Domain(ErrorType.Conflict, carResult.Status.ToString()));
        
        var car = carResult.Car;
        if (car is null)
            return Result<bool>.Failure(Error.Domain(ErrorType.Unknown, "No car"));

        var saved = await carRepository.AddCarAsync(car);
        if (!saved)
            return Result<bool>.Failure(Error.Application(ErrorType.Unknown, "Car not saved"));
        
        return Result<bool>.Success(true);
    }
}