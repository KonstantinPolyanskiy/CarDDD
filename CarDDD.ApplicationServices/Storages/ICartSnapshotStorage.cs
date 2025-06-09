using CarDDD.ApplicationServices.Models.StorageModels;

namespace CarDDD.ApplicationServices.Storages;

public interface ICartSnapshotStorage
{
    public Task<bool> WriteAsync(CartSnapshot cartSnapshot, CancellationToken ct = default);
    
    public Task<CartSnapshot?> ReadByIdAsync(Guid cartId, CancellationToken ct = default);
    public Task<CartSnapshot?> ReadByCustomerId(Guid customerId, CancellationToken ct = default);
}