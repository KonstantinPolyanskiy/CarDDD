namespace CarDDD.DomainServices.ValueObjects;

public readonly record struct Employer(EmployerId Id, IReadOnlyList<Role> Roles)
{
    public static Employer From(Guid id, IReadOnlyList<Role> roles) => new(EmployerId.From(id), roles);
    
    public static Employer From(EmployerId id, IReadOnlyList<Role> roles) => new(id, roles);
}