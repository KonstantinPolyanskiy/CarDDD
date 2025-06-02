using CarDDD.Core.DomainObjects.DomainCart;

namespace CarDDD.Infrastructure.Repositories;

public interface ICartRepository
{
    public Task<Cart?> GetByIdAsync(Guid id);
    public Task<bool> AddAsync (Cart cart);
}