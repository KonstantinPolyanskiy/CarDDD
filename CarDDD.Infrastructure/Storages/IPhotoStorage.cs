using CarDDD.Core.SnapshotModels;

namespace CarDDD.Infrastructure.Storages;

public interface IPhotoStorage
{
    public Task<bool> SavePhotoSnapshot(PhotoSnapshot snapshot);
    
    public Task<PhotoSnapshot?> GetPhotoSnapshot(Guid photoId);
}