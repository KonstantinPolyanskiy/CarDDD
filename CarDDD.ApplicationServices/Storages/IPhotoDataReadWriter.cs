using CarDDD.ApplicationServices.Models.StorageModels;

namespace CarDDD.ApplicationServices.Storages;

/// <summary>
/// Запись фото
/// </summary>
public interface IPhotoWriter
{
    public Task<bool> WriteAsync(PhotoData d, CancellationToken ct = default);
}

/// <summary>
/// Чтение фото
/// </summary>
public interface IPhotoReader
{
    public Task<PhotoData?> ReadOneAsync(Guid photoId, CancellationToken ct = default);
}