namespace CarDDD.DomainServices.EntityObjects;

/// <summary> Однозначно идентифицируемая сущность в рамках бизнеса </summary> 
public abstract class Entity<TId> 
{
    public Guid EntityId { get; set; } = Guid.NewGuid();
}