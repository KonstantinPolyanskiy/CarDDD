using CarDDD.Infrastructure.Models.SnapshotModels;
using Microsoft.Extensions.Logging;
using Minio;
using Minio.DataModel.Args;

namespace CarDDD.Infrastructure.Storages.Implementations;

public class MinioPhotoStorage(IMinioClient minio, ILogger<MinioPhotoStorage> log) : IPhotoStorage
{
    private const string Bucket = "PhotoBucket";

    public async Task<bool> SavePhotoSnapshot(PhotoSnapshot snapshot)
    {
        
    }

    public async Task<PhotoSnapshot?> GetPhotoSnapshot(Guid photoId)
    {
        
    }
    
    
}

