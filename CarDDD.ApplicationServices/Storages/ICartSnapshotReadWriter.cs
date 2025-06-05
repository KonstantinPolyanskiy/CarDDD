using CarDDD.ApplicationServices.Models.StorageModels;

namespace CarDDD.ApplicationServices.Storages;

/// <summary>
/// Чтение из хранилища данных модели Корзина
/// </summary>
public interface ICartSnapshotReader
{
    /// <summary> Читает из хранилища данные машины по Id и возвращает снимок. Null - если не найдено </summary>  
    public Task<CartSnapshot?> ReadOneAsync(Guid cartId, CancellationToken cancellationToken = default);
}

/// <summary>
/// Запись в хранилище данных модели Корзина
/// </summary>
public interface ICartSnapshotWriter
{
    /// <summary> Пишет в хранилище данные корзины. True - если сохранено </summary>
    public Task<bool> WriteAsync(CartSnapshot s, CancellationToken cancellationToken = default);
}

public interface ICartSnapshotReaderWriter
{
    public ICartSnapshotReader Reader { get; }
    public ICartSnapshotWriter Writer { get; }
}