using System.Collections.Concurrent;
using CarDDD.Core.DomainObjects.DomainCart;
using Microsoft.Extensions.Logging;

namespace CarDDD.Infrastructure.Repositories.Implementations;

public class MemoryCartRepository(ILogger<MemoryCartRepository> log) : ICartRepository
{
    private readonly ConcurrentDictionary<Guid, Cart> _store = new();

    public async Task<Cart?> GetByIdAsync(Guid id)
    {
        try
        {
            _store.TryGetValue(id, out var cart);
            return await Task.FromResult(cart);
        }
        catch (Exception ex)
        {
            log.LogError(ex, ex.Message);
            return await Task.FromResult<Cart?>(null);
        }
    }

    public async Task<bool> AddAsync(Cart cart)
    {
        try
        {
            if (!_store.TryAdd(cart.EntityId, cart))
                return await Task.FromResult(false);

            return await Task.FromResult(true);
        }
        catch (Exception ex)
        {
            log.LogError(ex, ex.Message);
            return await Task.FromResult(false);
        }
    }
}
