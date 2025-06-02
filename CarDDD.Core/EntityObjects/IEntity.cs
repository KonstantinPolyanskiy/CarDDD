using CarDDD.Core.DomainEvents;

namespace CarDDD.Core.EntityObjects;

public abstract class Entity<TId> 
{
    public Guid EntityId { get; set; } = Guid.NewGuid();
}