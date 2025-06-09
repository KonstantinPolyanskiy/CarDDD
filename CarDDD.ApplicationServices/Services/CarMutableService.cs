using AutoMapper;
using CarDDD.ApplicationServices.Dispatchers;
using CarDDD.ApplicationServices.Models.AnswerObjects.Result;
using CarDDD.ApplicationServices.Models.AnswerObjects.ServiceResponses;
using CarDDD.ApplicationServices.Models.RequestModels;
using CarDDD.ApplicationServices.Models.StorageModels;
using CarDDD.ApplicationServices.Repositories;
using CarDDD.ApplicationServices.Storages;
using CarDDD.DomainServices.DomainAggregates.CarAggregate;
using CarDDD.DomainServices.DomainAggregates.CarAggregate.Results;
using CarDDD.DomainServices.Services;
using CarDDD.DomainServices.Specifications;
using CarDDD.DomainServices.ValueObjects;
using Car = CarDDD.DomainServices.DomainAggregates.CarAggregate.Car;

namespace CarDDD.ApplicationServices.Services;

/// <summary>
/// Application сервис для добавления/изменения доменной модели Машина
/// </summary>
public interface ICarMutableService 
{
    public Task<Result<CarInfo>> AddNewCarAsync(CreateCarRequest req, CancellationToken ct = default);
    public Task<Result<CarInfo>> UpdateCarAsync(EditCarRequest req, CancellationToken ct = default);
    public Task<Result<bool>> SellCarAsync(SellCarRequest req, CancellationToken ct = default);
}

/// <summary>
/// Сервис для изменения состояния машин,  
/// </summary>
public class CarMutableService(ICarDomainService carService, ICarRepository carRepository, ITemporaryPhotoStorage temporarily,
    IDomainEventDispatcher domainDispatcher, IMapper mapper) : ICarMutableService
{
    public async Task<Result<CarInfo>> AddNewCarAsync(CreateCarRequest req, CancellationToken ct = default)
    {
        var carSpec = new CreateCarSpec
        {
            Brand = req.Brand,
            Color = req.Color,
            Price = req.Price,
            Mileage = req.Mileage,
            Photo = RequestHavePhoto(req) 
                ? new AttachCarPhotoSpec {Extension = req.PhotoExtension, OnlyIfNonePhoto = true}
                : null,
            Manager = Employer.From(req.EmployerId, req.EmployerRoles)
        };
        
        var created = carService.CreateCar(carSpec);
        switch (created.Status)
        {
            case CreateCarAction.ErrorAttachPhoto:
                return Result<CarInfo>.Failure(Error.Domain(ErrorType.Conflict, "error attaching photo"));
            case CreateCarAction.ErrorInvalidPrice:
                return Result<CarInfo>.Failure(Error.Domain(ErrorType.Conflict, "invalid price"));
        }

        var added = await carRepository.AddAsync(created.Car, ct);
        if (!added)
            return Result<CarInfo>.Failure(Error.Infrastructure(ErrorType.Conflict, "error saving car data"));
        
        // Если машина успешно создана сохранена - пишем фото во временное хранилище
        // Позже DomainHandler прочтет фото и запишет в основное хранилище
        if (created.Car.Photo.Attached())
            await temporarily.WriteAsync(new PhotoSnapshot
            {
                Id = created.Car.Photo.Id,
                Extension = created.Car.Photo.Extension,
                Data = req.PhotoData!
            }, ct);
        
        await domainDispatcher.DispatchAsync(created.Car.DomainEvents, ct);
        created.Car.ClearAllDomainEvents();

        return Result<CarInfo>.Success(
            mapper.Map<CarInfo>(created.Car)
        );
    }

    public async Task<Result<CarInfo>> UpdateCarAsync(EditCarRequest req, CancellationToken ct = default)
    {
        var car = await carRepository.GetAsync(CarId.From(req.CarId), ct);
        if (car == null)
            return Result<CarInfo>.Failure(Error.Application(ErrorType.NotFound, $"car by id: {req.CarId} not found"));

        var (spec, photoRewrite) = MapUpdateRequestToSpec(req, car);
        
        var updating = carService.UpdateCar(car, spec);
        if (updating.Status is not UpdateCarAction.Success)
        {
            switch (updating.Status)
            {
                case UpdateCarAction.ErrorInvalidPrice: 
                    return Result<CarInfo>.Failure(Error.Domain(ErrorType.Conflict, "invalid price"));
                case UpdateCarAction.ErrorEnoughPermission:
                    return Result<CarInfo>.Failure(Error.Domain(ErrorType.Forbidden, "enough permission"));
                case UpdateCarAction.ErrorPhotoAlreadyExists:
                    return Result<CarInfo>.Failure(Error.Domain(ErrorType.Conflict, "photo already exist"));
                case UpdateCarAction.ErrorPhotoNotAttached:
                    return Result<CarInfo>.Failure(Error.Domain(ErrorType.Conflict, "photo is not attached"));
                case UpdateCarAction.ErrorCreatingPhoto:
                    return Result<CarInfo>.Failure(Error.Domain(ErrorType.Conflict, "photo is not created"));
            }
        }
        
        var saved = await carRepository.UpdateAsync(car, ct);
        if (!saved)
            return Result<CarInfo>.Failure(Error.Application(ErrorType.Conflict, "error saving car data"));

        if (photoRewrite)
            await temporarily.WriteAsync(new PhotoSnapshot
            {
                Id = car.Photo.Id,
                Extension = spec.PhotoSpec!.Extension,
                Data = req.PhotoData!
            }, ct);
        
        await domainDispatcher.DispatchAsync(car.DomainEvents, ct);
        car.ClearAllDomainEvents();

        return Result<CarInfo>.Success(
            mapper.Map<CarInfo>(car)
        );
    }

    public async Task<Result<bool>> SellCarAsync(SellCarRequest req, CancellationToken ct = default)
    {
        if (req.CarId == Guid.Empty)
            return Result<bool>.Failure(Error.Application(ErrorType.Validation, "Nothing car to sell"));
        
        if (req.CustomerId == Guid.Empty)
            return Result<bool>.Failure(Error.Application(ErrorType.Validation, "Nothing customer to sell"));

        var car = await carRepository.GetAsync(CarId.From(req.CarId), ct);
        if (car == null)
            return Result<bool>.Failure(Error.Application(ErrorType.NotFound, "car not found"));

        var sold = carService.SellCar(car, new SellCarSpec { CustomerId = CustomerId.From(req.CustomerId) });
        if (sold.Status is not SellCarAction.Success)
        {
            switch (sold.Status)
            {
                case SellCarAction.ErrorIsNotAvailable:
                    return Result<bool>.Failure(Error.Domain(ErrorType.Conflict, "car is not available"));
                case SellCarAction.ErrorInvalidPrice:
                    return Result<bool>.Failure(Error.Domain(ErrorType.Conflict, "invalid price"));
            }
        }
        
        await domainDispatcher.DispatchAsync(car.DomainEvents, ct);
        car.ClearAllDomainEvents();
        
        return Result<bool>.Success(true);
    }

    private (UpdateCarSpec, bool) MapUpdateRequestToSpec(EditCarRequest req, Car car)
    {
        var updatePhoto = false;
        
        UpdateCarBasicAttributesSpec? basicSpec = null;
        if (req.Brand != null || req.Color != null || req.Price != null || req.Mileage != null)
        {
            basicSpec = new UpdateCarBasicAttributesSpec
            {
                Brand   = req.Brand   ?? car.Brand,
                Color   = req.Color   ?? car.Color,
                Price   = req.Price   ?? car.Price,
                Mileage = req.Mileage ?? car.Mileage
            };
        }
        
        AttachCarPhotoSpec? photoSpec = null;
        if (req.PhotoData != null && !string.IsNullOrWhiteSpace(req.PhotoExtension))
        {
            photoSpec = new AttachCarPhotoSpec
            {
                Extension       = req.PhotoExtension,
                OnlyIfNonePhoto = req.OnlyIfNonePhoto
            };
            updatePhoto = true;
        }
        
        AssignCarManagerSpec? assignSpec = null;
        if (req.NewManagerId.HasValue && req.NewManagerId.Value != Guid.Empty)
        {
            assignSpec = new AssignCarManagerSpec
            {
                AssignmentManager = new Employer
                {
                    Id = EmployerId.From(req.EmployerId),
                    Roles    = req.NewManagerRoles ?? []
                },
            };
        }
        
        return (new UpdateCarSpec
        {
            BasisAttributesSpec = basicSpec,
            PhotoSpec = photoSpec,
            AssignManagerSpec = assignSpec,
            Employer = new Employer { Id = EmployerId.From(req.EmployerId), Roles = req.EmployerRoles }
        }, updatePhoto);
    }

    private bool RequestHavePhoto(CreateCarRequest req)
        => req.PhotoData != null && !string.IsNullOrWhiteSpace(req.PhotoExtension);
}