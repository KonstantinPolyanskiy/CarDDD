using CarDDD.Core.EntityObjects;

namespace CarDDD.Core.DomainObjects.DomainCar;

public sealed class Employer : Entity<Guid>
{
    public static Employer From(Guid id, IReadOnlyList<Role> roles) => new()
        {Roles = roles};
    
    public IReadOnlyList<Role> Roles { get; private init; } = [];
}

public sealed class Consumer
{
    public static Consumer From(Guid id, string firstName, string lastName) => new()
        { Id = id, FirstName = firstName, LastName = lastName };
    
    public Guid Id { get; private init; }
    public string FirstName { get; private init; } = string.Empty;
    public string LastName { get; private init; } = string.Empty;
}