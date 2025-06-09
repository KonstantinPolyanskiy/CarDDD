using System.Collections.Concurrent;
using CarDDD.ApplicationServices.Models.StorageModels;
using CarDDD.ApplicationServices.Storages;

namespace CarDDD.Infrastructure.Storages;

/// <summary>
/// Реализует временную запись и чтение фото в in memory
/// </summary>
public class MemoryTemporaryPhotoStorage : ITemporaryPhotoStorage
{
    private readonly ConcurrentDictionary<Guid, PhotoSnapshot> _buffer = new();
    
    public Task<PhotoSnapshot?> ReadAsync(Guid photoId, CancellationToken ct = default)
    {
        return _buffer.TryRemove(photoId, out var data) ? Task.FromResult<PhotoSnapshot?>(data) : Task.FromResult<PhotoSnapshot?>(null);    
    }

    public Task<string?> DownloadUrlAsync(Guid photoId, CancellationToken ct = default)
    {
        return Task.FromResult<string?>(null);
    }

    public Task<bool> WriteAsync(PhotoSnapshot d, CancellationToken ct = default)
    {
        return Task.FromResult(_buffer.TryAdd(d.Id, d));
    }
}