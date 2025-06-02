using CarDDD.Core.SnapshotModels;
using Microsoft.Extensions.Logging;
using Minio;
using Minio.DataModel.Args;

namespace CarDDD.Infrastructure.Storages.Implementations;

public class MinioPhotoStorage(IMinioClient minio, ILogger<MinioPhotoStorage> log) : IPhotoStorage
{
    private const string Bucket = "PhotoBucket";

    public async Task<bool> SavePhotoSnapshot(PhotoSnapshot snapshot)
    {
        try
        {
            await CheckAndCreateBucket(Bucket);
            
            await using var ms = new MemoryStream(snapshot.Data);
            
            var args = new PutObjectArgs()
                .WithBucket(Bucket)
                .WithObject(snapshot.Id.ToString())
                .WithStreamData(ms)
                .WithObjectSize(ms.Length)
                .WithContentType("image/" + snapshot.Extension);

            await minio.PutObjectAsync(args);
            
            return true;
        }
        catch (Exception ex)
        {
            log.LogError(ex, ex.Message);
            return false;
        }
    }

    public async Task<PhotoSnapshot?> GetPhotoSnapshot(Guid photoId)
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
    
    private async Task CheckAndCreateBucket(string bucketName)
    {
        var exist = await minio.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucketName));
        if (exist is false)
            await minio.MakeBucketAsync(new MakeBucketArgs().WithBucket(bucketName));
    }
}

