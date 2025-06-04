using CarDDD.Core.EntityObjects;

namespace CarDDD.Core.DomainObjects.DomainCar.EntityObjects;

public sealed class Employer : Entity<Guid>
{
    public static Employer From(Guid id, IReadOnlyList<Role> roles) => new()
        { EntityId = id, Roles = roles};
    
    public IReadOnlyList<Role> Roles { get; private init; } = [];

    public bool IsAdmin => Roles.Contains(Role.CarAdmin);

    public override bool Equals(object? obj)
    {
        if (obj is Employer employer)
            return employer.EntityId == EntityId;
        
        if (obj is Manager manager)
            return EntityId == manager.Id;
        
        return false;
    }
}

