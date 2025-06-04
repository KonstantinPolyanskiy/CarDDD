namespace CarDDD.DomainServices.ValueObjects;

public readonly record struct ManagerId(Guid Value)
{
    public static ManagerId From(Guid id) => new(id);
}

public readonly record struct EmployerId(Guid Value)
{
    public static EmployerId From(Guid id) => new(id);
}

public readonly record struct CarId(Guid Value)
{
    public static CarId From(Guid id) => new(id);
}

public readonly record struct CustomerId(Guid Value)
{
    public static CustomerId From(Guid id) => new(id);
}

public readonly record struct PhotoId(Guid Value)
{
    public static PhotoId From(Guid id) => new(id);
}

public readonly record struct CartId(Guid Value)
{
    public static CartId From(Guid id) => new(id);
}