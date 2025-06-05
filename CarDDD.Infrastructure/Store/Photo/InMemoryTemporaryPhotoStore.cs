using System.Collections.Concurrent;
using CarDDD.ApplicationServices.Models.StorageModels;
using CarDDD.ApplicationServices.Storages;

namespace CarDDD.Infrastructure.Store.Photo;

/// <summary>
/// Реализует временную запись и чтение фото в in memory. <see cref="ReadOneAsync"/> удаляет фото по guid после чтения
/// </summary>
public class InMemoryTemporaryPhotoStore : IPhotoReader, IPhotoWriter
{
    /// <summary>
    /// Замена <see cref="IPhotoStorage"/> для хранения данных в памяти
    /// </summary>
    private readonly ConcurrentDictionary<Guid, PhotoData> _buffer = new();
    
    public Task<PhotoData?> ReadOneAsync(Guid photoId, CancellationToken ct = default)
    {
        return _buffer.TryRemove(photoId, out var data) ? Task.FromResult<PhotoData?>(data) : Task.FromResult<PhotoData?>(null);
    }

    public Task<bool> WriteAsync(PhotoData d, CancellationToken ct = default)
    {
        return Task.FromResult(_buffer.TryAdd(d.Id, d));
    }
}