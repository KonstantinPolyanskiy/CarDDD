namespace CarDDD.Core.DomainObjects.CommonValueObjects;

public record CarId
{
    public Guid Value { get; private init; }
    public static CarId From(Guid value) => new() { Value = value };
}

public record ConsumerId
{
    public Guid Value { get; private init; }
    public static ConsumerId From(Guid value) => new() { Value = value };
}

public record CartId
{
    public Guid Value { get; private init; }
    public static CartId From(Guid value) => new() { Value = value };
}