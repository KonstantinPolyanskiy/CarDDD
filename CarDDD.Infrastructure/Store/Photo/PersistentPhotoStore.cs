using CarDDD.ApplicationServices.Models.StorageModels;
using CarDDD.ApplicationServices.Storages;
using Minio;

namespace CarDDD.Infrastructure.Store.Photo;

/// <summary>
/// Идейно сохраняет фото как можно более гарантированно
/// Если не получилось в главное (minio) - может послать в очередь, сохранить в fs, etc
/// </summary>
public class PersistentPhotoStore(IPhotoStorage minio, IPhotoStorage fileSystem) : IPhotoReader, IPhotoWriter
{
    public async Task<PhotoData?> ReadOneAsync(Guid photoId, CancellationToken ct = default)
    {
        var data = await minio.GetById(photoId, ct);
        if (data != null)
            return data;
        
        data = await minio.GetById(photoId, ct);
        if (data != null)
            return data;

        return null;
    }

    public async Task<bool> WriteAsync(PhotoData d, CancellationToken ct = default)
    {
        var savedMinio = await minio.Save(d, ct);
        if (savedMinio)
            return true;
        
        var savedFs = await fileSystem.Save(d, ct);
        
        return savedFs;
    }
}