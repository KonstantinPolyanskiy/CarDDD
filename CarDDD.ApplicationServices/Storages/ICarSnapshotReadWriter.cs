using CarDDD.ApplicationServices.Models.StorageModels;

namespace CarDDD.ApplicationServices.Storages;

/// <summary>
/// Чтение из хранилища данных модели Машина
/// </summary>
public interface ICarSnapshotReader
{
    /// <summary> Читает из хранилища данные машины по Id и возвращает снимок. Null - если не найдено </summary>  
    public Task<CarSnapshot?> ReadOneAsync(Guid carId, CancellationToken cancellationToken = default);
}

/// <summary>
/// Запись в хранилище данных модели Машина
/// </summary>
public interface ICarSnapshotWriter
{
    /// <summary> Пишет в хранилище данные машины. True - если сохранено </summary>
    public Task<bool> WriteAsync(CarSnapshot s, CancellationToken cancellationToken = default);
}

public interface ICarSnapshotReaderWriter
{
    public ICarSnapshotReader Reader { get; }
    public ICarSnapshotWriter Writer { get; }
}