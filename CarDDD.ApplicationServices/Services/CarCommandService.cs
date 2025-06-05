using CarDDD.ApplicationServices.Dispatchers;
using CarDDD.ApplicationServices.Models.AnswerObjects.Result;
using CarDDD.ApplicationServices.Models.RequestModels;
using CarDDD.ApplicationServices.Models.StorageModels;
using CarDDD.ApplicationServices.Repositories;
using CarDDD.ApplicationServices.Storages;
using CarDDD.DomainServices.DomainAggregates.CarAggregate;
using CarDDD.DomainServices.DomainAggregates.CarAggregate.Results;
using CarDDD.DomainServices.Services;
using CarDDD.DomainServices.Specifications;
using CarDDD.DomainServices.ValueObjects;

namespace CarDDD.ApplicationServices.Services;

/// <summary>
/// Сервис для изменения состояния машин,  
/// </summary>
public class CarCommandService(ICarDomainService carService, ICarRepository carRepository, IPhotoWriter temporarily,
    IDomainEventDispatcher domainDispatcher)
{
    public async Task<Result<bool>> AddNewCar(AddNewCarRequest req, CancellationToken ct = default)
    {
        AttachCarPhotoSpec? photoSpec = null;
        if (req.PhotoData != null && !string.IsNullOrEmpty(req.PhotoExtension))
            photoSpec = new AttachCarPhotoSpec { Extension = req.PhotoExtension, OnlyIfNonePhoto = true };
        
        var carSpec = new CreateCarSpec
        {
            Brand = req.Brand,
            Color = req.Color,
            Price = req.Price,
            Mileage = req.Mileage,
            Photo = photoSpec,
            Manager = Employer.From(req.EmployerId, req.EmployerRoles)
        };
        
        var created = carService.CreateCar(carSpec);
        switch (created.Status)
        {
            case CreateCarAction.ErrorAttachPhoto:
                return Result<bool>.Failure(Error.Domain(ErrorType.Conflict, "error attaching photo"));
            case CreateCarAction.ErrorInvalidPrice:
                return Result<bool>.Failure(Error.Domain(ErrorType.Conflict, "invalid price"));
        }

        var added = await carRepository.AddAsync(created.Car, ct);
        if (!added)
            return Result<bool>.Failure(Error.Infrastructure(ErrorType.Conflict, "error saving car data"));
        
        // Если машина успешно создана сохранена - пишем фото во временное хранилище
        // Позже DomainHandler прочтет фото и запишет в основное хранилище
        if (created.Car.Photo.Attached())
            await temporarily.WriteAsync(new PhotoData
            {
                Id = created.Car.Photo.Id,
                Extension = created.Car.Photo.Extension,
                Data = req.PhotoData!
            }, ct);
        
        await domainDispatcher.DispatchAsync(created.Car.DomainEvents, ct);
        created.Car.ClearAllDomainEvents();

        //TODO: use mapper and return car info 
        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> UpdateCar(UpdateCarRequest req, CancellationToken ct = default)
    {
        var car = await carRepository.GetOneAsync(CarId.From(req.CarId), ct);
        if (car == null)
            return Result<bool>.Failure(Error.Application(ErrorType.NotFound, "car not found"));

        var (spec, photoRewrite) = MapUpdateRequestToSpec(req, car);
        
        var updating = carService.UpdateCar(car, spec);
        if (updating.Status is not UpdateCarAction.Success)
        {
            switch (updating.Status)
            {
                case UpdateCarAction.ErrorInvalidPrice: 
                    return Result<bool>.Failure(Error.Domain(ErrorType.Conflict, "invalid price"));
                case UpdateCarAction.ErrorEnoughPermission:
                    return Result<bool>.Failure(Error.Domain(ErrorType.Forbidden, "enough permission"));
                case UpdateCarAction.ErrorPhotoAlreadyExists:
                    return Result<bool>.Failure(Error.Domain(ErrorType.Conflict, "photo already exist"));
                case UpdateCarAction.ErrorPhotoNotAttached:
                    return Result<bool>.Failure(Error.Domain(ErrorType.Conflict, "photo is not attached"));
                case UpdateCarAction.ErrorCreatingPhoto:
                    return Result<bool>.Failure(Error.Domain(ErrorType.Conflict, "photo is not created"));
            }
        }
        
        var saved = await carRepository.UpdateAsync(car, ct);
        if (!saved)
            return Result<bool>.Failure(Error.Application(ErrorType.Conflict, "error saving car data"));

        if (photoRewrite)
            await temporarily.WriteAsync(new PhotoData
            {
                Id = car.Photo.Id,
                Extension = spec.PhotoSpec!.Extension,
                Data = req.PhotoData!
            }, ct);
        
        await domainDispatcher.DispatchAsync(car.DomainEvents, ct);
        car.ClearAllDomainEvents();
        
        return Result<bool>.Success(true);
    }

    private (UpdateCarSpec, bool) MapUpdateRequestToSpec(UpdateCarRequest req, Car car)
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
}