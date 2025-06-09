using CarDDD.ApplicationServices.Models.StorageModels;

namespace CarDDD.ApplicationServices.Storages;

/// <summary>
/// Чтение и запись в абстрактное хранилище фотографий
/// </summary>
public interface IPhotoStorage
{
    public Task<PhotoSnapshot?> ReadAsync(Guid photoId, CancellationToken ct = default);
    public Task<string?> DownloadUrlAsync(Guid photoId, CancellationToken ct = default);
    
    public Task<bool> WriteAsync(PhotoSnapshot photo, CancellationToken ct = default);
}

public interface ITemporaryPhotoStorage : IPhotoStorage { }
public interface IMainPhotoStorage      : IPhotoStorage { }