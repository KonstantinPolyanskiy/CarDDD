using CarDDD.Core.DomainObjects;
using CarDDD.Core.DomainObjects.DomainCar;
using CarDDD.Core.EntityObjects;
using CarDDD.Core.SnapshotModels;
using CarDDD.Infrastructure.Contexts;
using Microsoft.Extensions.Logging;
using Minio;
using Minio.DataModel.Args;

namespace CarDDD.Infrastructure.Repositories.Implementations;

public class CarRepository(ApplicationDbContext database, IMinioClient minio, ILogger log) : ICarRepository
{
    private const string Bucket = "PhotoBucket";
    
    public async Task<bool> AddCarAsync(Car car)
    {
        try
        {
            log.LogInformation("Сохранение машины в репозиторий");

            var carSaved = await SaveCarSnapshotAsync(car);
            if (!carSaved)
                return false;
            
            var photoSaved = await SavePhotoSnapshotAsync(car.Photo);
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

            var carSaved = await UpdateCarAsync(car);
            if (!carSaved)
                return false;
            
            var photoSaved = await SavePhotoSnapshotAsync(car.Photo);
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
            log.LogInformation("Получение машины из репозитория");

            var carSnapshot = await GetCarSnapshotAsync(carId);
            if (carSnapshot == null)
                return null;

            var photoSnapshot = await GetPhotoSnapshotAsync((Guid)carSnapshot.PhotoId!);

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

    #region CarSnapshot

    private async Task<bool> SaveCarSnapshotAsync(Car car)
    {
        try
        {
            log.LogInformation("Сохранение снимка данных для машины {carId}", car.EntityId);
            
            var snapshot = new CarSnapshot
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
            };
            
            await database.Cars.AddAsync(snapshot);
            await database.SaveChangesAsync();
            
            return true;
        }
        catch (Exception ex)
        {
            log.LogError(ex, ex.Message);
            return false;
        }
    }

    private async Task<bool> UpdateCarSnapshotAsync(Car car)
    {
        try
        {
            log.LogError("Обновление снимка данных для машины {id}", car.EntityId);
            
            var snapshot = await database.Cars.FindAsync(car.EntityId);
            if (snapshot == null)
                return false;
            
            snapshot.Brand = car.Brand;
            snapshot.Color = car.Color;
            snapshot.Price = car.Price;
            
            snapshot.PhotoId = car.Photo.EntityId;
            snapshot.ManagerId = car.ResponsiveManagerId;
            
            snapshot.PreviousOwnerName = car.PreviousOwner.Name;
            snapshot.Mileage = car.PreviousOwner.Mileage;
            snapshot.Condition = car.Condition;
            snapshot.IsAvailable = car.IsAvailable;
            
            await database.SaveChangesAsync();
            
            return true;
        }
        catch (Exception ex)
        {
            log.LogError(ex, ex.Message);
            return false;
        }
    }

    private async Task<CarSnapshot?> GetCarSnapshotAsync(Guid carId)
    {
        try
        {
            log.LogInformation("Получение снимка данных для машины {id}", carId);

            var snapshot = await database.Cars.FindAsync(carId);

            return snapshot;
        }
        catch (Exception ex)
        {
            log.LogError(ex, ex.Message);
            return null;
        }
    }
    
    #endregion

    #region PhotoSnapshot
    
    private async Task<bool> SavePhotoSnapshotAsync(Photo photo)
    {
        try
        {
            log.LogInformation("Сохранение снимка данных для фото {photoId}", photo.EntityId);
            
            await CheckAndCreateBucket(Bucket);
            
            await using var ms = new MemoryStream(photo.Data);
            
            var args = new PutObjectArgs()
                .WithBucket(Bucket)
                .WithObject(photo.EntityId.ToString())
                .WithStreamData(ms)
                .WithObjectSize(ms.Length)
                .WithContentType("image/" + photo.Extension);

            await minio.PutObjectAsync(args);
            
            return true;
        }
        catch (Exception ex)
        {
            log.LogError(ex, ex.Message);
            return false;
        }
    }

    private async Task<PhotoSnapshot?> GetPhotoSnapshotAsync(Guid photoId)
    {
        try
        {
            log.LogInformation("Получение снимка данных для фото {id}", photoId);

            var stat = await minio.StatObjectAsync(
                new StatObjectArgs()
                    .WithBucket(Bucket)
                    .WithObject(photoId.ToString()));

            stat.MetaData.TryGetValue("x-amz-meta-ext", out var ext);
            var extension = !string.IsNullOrWhiteSpace(ext)
                ? ext
                : stat.ContentType?.Split('/').Last() ?? "bin";

            await using var ms = new MemoryStream();
            await minio.GetObjectAsync(
                new GetObjectArgs()
                    .WithBucket(Bucket)
                    .WithObject(photoId.ToString())
                    .WithCallbackStream(async s => await s.CopyToAsync(ms)));

            return new PhotoSnapshot
            {
                Id = photoId,
                Extension = extension,
                Data = ms.ToArray()
            };
        }
        catch (Exception ex)
        {
            log.LogError(ex, ex.Message);
            return null;
        }
    }
    
    #endregion
    
    private async Task CheckAndCreateBucket(string bucketName)
    {
        var exist = await minio.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucketName));
        if (exist is false)
            await minio.MakeBucketAsync(new MakeBucketArgs().WithBucket(bucketName));
    }
}