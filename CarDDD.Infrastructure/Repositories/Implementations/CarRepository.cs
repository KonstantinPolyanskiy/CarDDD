using CarDDD.Core.AnswerObjects.Result;
using CarDDD.Core.DomainObjects.DomainCar;
using CarDDD.Core.DtoObjects;
using CarDDD.Core.SnapshotModels;
using CarDDD.Infrastructure.Storages;
using Microsoft.Extensions.Logging;

namespace CarDDD.Infrastructure.Repositories.Implementations;

public class CarRepository(ICarStorage cars, IPhotoStorage photos,
    ILogger log) : ICarRepository
{
    
    public async Task<bool> AddCarAsync(Car car)
    {
        try
        {
            var carSaved = await cars.SaveCarSnapshotAsync(new CarSnapshot
            {
                Id = car.EntityId,
                PhotoId = car.Photo.EntityId,
                ManagerId = car.ResponsiveManagerId,
                Brand = car.Brand,
                Color = car.Color,
                Price = car.Price,
                PreviousOwnerName = car.PreviousOwner.Name,
                Mileage = car.PreviousOwner.Mileage,
                Condition = car.Condition,
                IsAvailable = car.IsAvailable,
            });
            if (!carSaved)
                return false;
            
            var photoSaved = await photos.SavePhotoSnapshot(new PhotoSnapshot
            {
                Id = car.Photo.EntityId,
                Extension = car.Photo.Extension,
                Data = car.Photo.Data
            });
            if (!photoSaved)
                return false;
            
            return true;
        }
        catch (Exception ex)
        {
            log.LogError(ex, ex.Message);
            return false;
        }
    }

    public async Task<bool> UpdateCarAsync(Car car)
    {
        try
        {
            log.LogInformation("Обновление машины в репозитории");

            var carSaved = await cars.UpdateCarSnapshot(new CarSnapshot
            {
                Id = car.EntityId,
                PhotoId = car.Photo.EntityId,
                ManagerId = car.ResponsiveManagerId,
                Brand = car.Brand,
                Color = car.Color,
                Price = car.Price,
                PreviousOwnerName = car.PreviousOwner.Name,
                Mileage = car.PreviousOwner.Mileage,
                Condition = car.Condition,
                IsAvailable = car.IsAvailable,
            });
            if (!carSaved)
                return false;
            
            var photoSaved = await photos.SavePhotoSnapshot(new PhotoSnapshot
            {
                Id = car.Photo.EntityId,
                Extension = car.Photo.Extension,
                Data = car.Photo.Data
            });
            if (!photoSaved)
                return false;
            
            return true;
        }
        catch (Exception ex)
        {
            log.LogError(ex, ex.Message);
            return false;
        }
    }
    
    public async Task<Car?> FindByIdAsync(Guid carId)
    {
        try
        {
            var carSnapshot = await cars.GetCarSnapshotAsync(carId);
            if (carSnapshot == null)
                return null;

            var photoSnapshot = await photos.GetPhotoSnapshot((Guid)carSnapshot.PhotoId!);

            return Car.Restore(
                carSnapshot.Id,
                carSnapshot.Brand,
                carSnapshot.Color,
                carSnapshot.Price,
                carSnapshot.Condition,
                Ownership.PreviousOwnership(carSnapshot.PreviousOwnerName, carSnapshot.Mileage),
                photoSnapshot is null ? Photo.None : Photo.From(photoSnapshot.Extension, photoSnapshot.Data),
                carSnapshot.IsAvailable,
                carSnapshot.ManagerId
            );
        }
        catch (Exception ex)
        {
            log.LogError(ex, ex.Message);
            return null;
        }
    }

    public async Task<PageResult<Car>?> FindByParams(SearchCarDto dto)
    {
        try
        {
            var carSnapPage = await cars.GetCarSnapshotsAsync(dto);
            if (carSnapPage == null)
                return null;

            var domainCars = new List<Car>(carSnapPage.PageSize);
            
            foreach (var s in carSnapPage.Items)
            {
                PhotoSnapshot? pSnap = s.PhotoId is null
                    ? null
                    : await photos.GetPhotoSnapshot(s.PhotoId.Value);

                var photo = pSnap is null
                    ? Photo.None
                    : Photo.From(pSnap.Extension, pSnap.Data);

                var car = Car.Restore(
                    s.Id,
                    s.Brand,
                    s.Color,
                    s.Price,
                    s.Condition,
                    Ownership.PreviousOwnership(s.PreviousOwnerName, s.Mileage),
                    photo,
                    s.IsAvailable,
                    s.ManagerId);

                domainCars.Add(car);
            }

            return await PageResult<Car>.CreateAsync(domainCars.AsQueryable(), carSnapPage.PageNumber, carSnapPage.PageSize);
        }
        catch (Exception ex)
        {
            log.LogError(ex, ex.Message);
            return null;
        }
    }
}