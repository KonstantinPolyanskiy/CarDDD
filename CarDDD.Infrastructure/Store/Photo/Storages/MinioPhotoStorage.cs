using CarDDD.ApplicationServices.Models.StorageModels;
using CarDDD.ApplicationServices.Storages;
using Microsoft.Extensions.Logging;
using Minio;
using Minio.DataModel.Args;

namespace CarDDD.Infrastructure.Store.Photo.Storages;

public class MinioPhotoStorage(IMinioClient client, ILogger<MinioPhotoStorage> log) : IPhotoStorage
{
    private const string Bucket = "PhotoBucket";
    
    public async Task<bool> Save(PhotoData d, CancellationToken ct = default)
    {
        try
        {
            await CheckAndCreateBucket(Bucket);
            
            await using var ms = new MemoryStream(d.Data);
            
            var args = new PutObjectArgs()
                .WithBucket(Bucket)
                .WithObject(d.Id.ToString())
                .WithStreamData(ms)
                .WithObjectSize(ms.Length)
                .WithContentType("image/" + d.Extension);

            await client.PutObjectAsync(args, ct);
            
            return true;
        }
        catch (Exception ex)
        {
            log.LogError(ex, ex.Message);
            return false;
        }
    }

    public async Task<PhotoData?> GetById(Guid photoId, CancellationToken ct = default)
    {
        try
        {

            var stat = await client.StatObjectAsync(
                new StatObjectArgs()
                    .WithBucket(Bucket)
                    .WithObject(photoId.ToString()),
                ct);

            stat.MetaData.TryGetValue("x-amz-meta-ext", out var ext);
            var extension = !string.IsNullOrWhiteSpace(ext)
                ? ext
                : stat.ContentType?.Split('/').Last() ?? "bin";

            await using var ms = new MemoryStream();
            await client.GetObjectAsync(
                new GetObjectArgs()
                    .WithBucket(Bucket)
                    .WithObject(photoId.ToString())
                    .WithCallbackStream(async s => await s.CopyToAsync(ms)));

            return new PhotoData
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
        var exist = await client.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucketName));
        if (exist is false)
            await client.MakeBucketAsync(new MakeBucketArgs().WithBucket(bucketName));
    }
}