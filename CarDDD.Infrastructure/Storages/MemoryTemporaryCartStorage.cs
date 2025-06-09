using System.Collections.Concurrent;
using CarDDD.ApplicationServices.Models.StorageModels;
using CarDDD.ApplicationServices.Storages;

namespace CarDDD.Infrastructure.Storages;

public class MemoryTemporaryCartStorage : ICartSnapshotStorage
{
    private readonly ConcurrentDictionary<Guid, CartSnapshot> _buffer = new();
    public Task<bool> WriteAsync(CartSnapshot cartSnapshot, CancellationToken ct = default)
    {
        return Task.FromResult(_buffer.TryAdd(cartSnapshot.Id, cartSnapshot));
    }

    public Task<CartSnapshot?> ReadByIdAsync(Guid cartId, CancellationToken ct = default)
    {
        _buffer.TryGetValue(cartId, out var cartSnapshot);
        
        return Task.FromResult(cartSnapshot);
    }

    public Task<CartSnapshot?> ReadByCustomerId(Guid customerId, CancellationToken ct = default)
    {
        foreach (var pair in _buffer)
        {
            if (pair.Value.CartOwnerId == customerId)
                return Task.FromResult(pair.Value)!;
        }
        
        return Task.FromResult<CartSnapshot?>(null);
    }
}