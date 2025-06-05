using CarDDD.ApplicationServices.Models.StorageModels;

namespace CarDDD.ApplicationServices.Storages;

public interface IPhotoStorage
{
    public Task<bool> Save(PhotoData d, CancellationToken ct = default);
    public Task<PhotoData?> GetById(Guid photoId, CancellationToken ct = default);
}