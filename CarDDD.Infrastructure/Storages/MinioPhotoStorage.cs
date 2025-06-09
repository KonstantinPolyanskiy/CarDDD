using CarDDD.ApplicationServices.Models.StorageModels;
using CarDDD.ApplicationServices.Storages;
using Microsoft.Extensions.Logging;
using Minio;
using Minio.DataModel.Args;

namespace CarDDD.Infrastructure.Storages;

public class MinioPhotoStorage(IMinioClient minio, ILogger<MinioPhotoStorage> log) : IMainPhotoStorage
{
    private const string Bucket = "PhotoBucket";

    public async Task<PhotoSnapshot?> ReadAsync(Guid photoId, CancellationToken ct = default)
    {
        try
        {
            var stat = await minio.StatObjectAsync(
                new StatObjectArgs()
                    .WithBucket(Bucket)
                    .WithObject(photoId.ToString()),
                ct);

            stat.MetaData.TryGetValue("x-amz-meta-ext", out var ext);
            var extension = !string.IsNullOrWhiteSpace(ext)
                ? ext
                : stat.ContentType?.Split('/').Last() ?? "bin";

            await using var ms = new MemoryStream();
            
            await minio.GetObjectAsync(
                new GetObjectArgs()
                    .WithBucket(Bucket)
                    .WithObject(photoId.ToString())
                    .WithCallbackStream(async s => await s.CopyToAsync(ms, ct)), ct);

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

    public async Task<string?> DownloadUrlAsync(Guid photoId, CancellationToken ct = default)
    {
        try
        {
            var objectName = photoId.ToString();

            var args = new PresignedGetObjectArgs()
                .WithBucket(Bucket)
                .WithObject(objectName)
                .WithExpiry(3200);

            return await minio.PresignedGetObjectAsync(args);
        }
        catch (Exception ex)
        {
            log.LogError(ex, ex.Message);
            return null;
        }
    }

    public async Task<bool> WriteAsync(PhotoSnapshot photo, CancellationToken ct = default)
    {
        try
        {
            await CheckAndCreateBucket(Bucket);
            
            await using var ms = new MemoryStream(photo.Data);
            
            var args = new PutObjectArgs()
                .WithBucket(Bucket)
                .WithObject(photo.Id.ToString())
                .WithStreamData(ms)
                .WithObjectSize(ms.Length)
                .WithContentType("image/" + photo.Extension);

            await minio.PutObjectAsync(args, ct);
            
            return true;
        }
        catch (Exception ex)
        {
            log.LogError(ex, ex.Message);
            return false;
        }
    }
    
    private async Task CheckAndCreateBucket(string bucketName)
    {
        var exist = await minio.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucketName));
        if (exist is false)
            await minio.MakeBucketAsync(new MakeBucketArgs().WithBucket(bucketName));
    }
}

