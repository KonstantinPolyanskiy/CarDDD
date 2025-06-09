using CarDDD.ApplicationServices.Models.StorageModels;

namespace CarDDD.ApplicationServices.Storages;

/// <summary>
/// Чтение и запись из хранилища данных машин
/// </summary>
public interface ICarSnapshotStorage
{
    public Task<CarSnapshot?> ReadAsync(Guid carId, CancellationToken ct = default);
    public Task<IQueryable<CarSnapshot>> QueryForReadAllAsync(CancellationToken ct = default);
    
    public Task<bool> SaveAsync(CarSnapshot carSnapshot, CancellationToken ct = default);
    public Task<bool> UpdateAsync(CarSnapshot newSnapshot, CancellationToken ct = default);
}