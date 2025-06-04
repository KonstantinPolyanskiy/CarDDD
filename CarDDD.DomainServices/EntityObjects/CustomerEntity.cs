namespace CarDDD.DomainServices.EntityObjects;

public sealed class Customer : Entity<Guid>
{
    public static Customer From(Guid customerId) => new() { EntityId = customerId};
    
}