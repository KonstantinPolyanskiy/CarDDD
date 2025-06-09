using CarDDD.ApplicationServices.Models.StorageModels;
using CarDDD.ApplicationServices.Storages;
using CarDDD.DomainServices.DomainAggregates.CartAggregate;
using CarDDD.DomainServices.ValueObjects;

namespace CarDDD.ApplicationServices.Repositories;

public interface ICartRepository
{
    public Task<bool> AddAsync(Cart cart, CancellationToken ct = default);
    public Task<bool> UpdateAsync(Cart cart, CancellationToken ct = default);
    
    public Task<Cart?> GetAsync(CartId cartId, CancellationToken ct = default);
    public Task<Cart?> GetAsync(CustomerId customerId, CancellationToken ct = default); 
}

public class CartRepository(ICartSnapshotStorage snapshots) : ICartRepository
{
    public async Task<bool> AddAsync(Cart cart, CancellationToken ct = default)
    {
        var cartSnapshot = new CartSnapshot
        {
            Id = cart.EntityId,
            Cars = cart.Cars.Select(x => x.CarId.Value).ToList(),
            CartOwnerId = cart.CartOwnerId.Value,
            Ordered = cart.Ordered,
            ReadyForPurchase = cart.ReadyForPurchase,
            Purchased = cart.Purchased,
        };
        
        return await snapshots.WriteAsync(cartSnapshot, ct);
    }

    public async Task<bool> UpdateAsync(Cart cart, CancellationToken ct = default)
    {
        var snapshot = await snapshots.ReadByIdAsync(cart.EntityId, ct);
        if (snapshot is null) 
            return false;

        snapshot.Cars = cart.Cars.Select(x => x.CarId.Value).ToList();
        snapshot.CartOwnerId = cart.CartOwnerId.Value;
        snapshot.Ordered = cart.Ordered;
        snapshot.ReadyForPurchase = cart.ReadyForPurchase;
        snapshot.Purchased = cart.Purchased;
        
        return await snapshots.WriteAsync(snapshot, ct);
    }

    public async Task<Cart?> GetAsync(CartId cartId, CancellationToken ct = default)
    {
        var snapshot = await snapshots.ReadByIdAsync(cartId.Value, ct);
        if (snapshot == null)
            return null;

        return Cart.Restore(
            snapshot.Id,
            snapshot.Cars.Select(c => new Car(CarId.From(c))),
            CustomerId.From(snapshot.CartOwnerId), 
            snapshot.Ordered,
            snapshot.ReadyForPurchase,
            snapshot.Purchased
        );
    }

    public async Task<Cart?> GetAsync(CustomerId customerId, CancellationToken ct = default)
    {
        var snapshot = await snapshots.ReadByCustomerId(customerId.Value, ct);
        if (snapshot == null)
            return null;
        
        return Cart.Restore(
            snapshot.Id,
            snapshot.Cars.Select(c => new Car(CarId.From(c))),
            CustomerId.From(snapshot.CartOwnerId), 
            snapshot.Ordered,
            snapshot.ReadyForPurchase,
            snapshot.Purchased
        );
    }
}