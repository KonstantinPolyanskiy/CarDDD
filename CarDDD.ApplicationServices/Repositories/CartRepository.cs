using CarDDD.ApplicationServices.Models.StorageModels;
using CarDDD.ApplicationServices.Storages;
using CarDDD.DomainServices.DomainAggregates.CartAggregate;
using CarDDD.DomainServices.ValueObjects;

namespace CarDDD.ApplicationServices.Repositories;

public interface ICartRepository
{
    public Task<bool> AddAsync(Cart cart, CancellationToken ct = default);
    public Task<bool> UpdateAsync(Cart cart, CancellationToken ct = default);
    
    public Task<Cart?> GetOneAsync(CartId cartId, CancellationToken ct = default);
}

public class CartRepository(ICartSnapshotReaderWriter snapshots) : ICartRepository
{
    public async Task<bool> AddAsync(Cart cart, CancellationToken ct = default)
    {
        var cartSnapshot = new CartSnapshot
        {
            Id = cart.EntityId,
            Cars = cart.Cars.Select(x => x.Value).ToList(),
            CartOwnerId = cart.CartOwnerId.Value,
            Ordered = cart.Ordered,
            ReadyForPurchase = cart.ReadyForPurchase,
            Purchased = cart.Purchased,
        };
        
        return await snapshots.Writer.WriteAsync(cartSnapshot, ct);
    }

    public async Task<bool> UpdateAsync(Cart cart, CancellationToken ct = default)
    {
        var snapshot = await snapshots.Reader.ReadOneAsync(cart.EntityId, ct);
        if (snapshot is null) 
            return false;

        snapshot.Cars = cart.Cars.Select(x => x.Value).ToList();
        snapshot.CartOwnerId = cart.CartOwnerId.Value;
        snapshot.Ordered = cart.Ordered;
        snapshot.ReadyForPurchase = cart.ReadyForPurchase;
        snapshot.Purchased = cart.Purchased;
        
        return await snapshots.Writer.WriteAsync(snapshot, ct);
    }

    public async Task<Cart?> GetOneAsync(CartId cartId, CancellationToken ct = default)
    {
        var snapshot = await snapshots.Reader.ReadOneAsync(cartId.Value, ct);
        if (snapshot == null)
            return null;

        return Cart.Restore(default, null, default, false, false, false);
    }
}