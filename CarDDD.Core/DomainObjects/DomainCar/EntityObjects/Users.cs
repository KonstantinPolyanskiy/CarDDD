using CarDDD.Core.EntityObjects;

namespace CarDDD.Core.DomainObjects.DomainCar.EntityObjects;

public sealed class Employer : Entity<Guid>
{
    public static Employer From(Guid id, IReadOnlyList<Role> roles) => new()
        {Roles = roles};
    
    public IReadOnlyList<Role> Roles { get; private init; } = [];
}

