using CarDDD.ApplicationServices.Models.StorageModels;
using CarDDD.ApplicationServices.Storages;

namespace CarDDD.Infrastructure.Store.Photo.Storages;

public class StubFileSystemPhotoStorage : IPhotoStorage
{
    public Task<bool> Save(PhotoData d, CancellationToken ct = default)
    {
        return Task.FromResult(false);
    }

    public Task<PhotoData?> GetById(Guid photoId, CancellationToken ct = default)
    {
        return Task.FromResult<PhotoData?>(null);
    }
}