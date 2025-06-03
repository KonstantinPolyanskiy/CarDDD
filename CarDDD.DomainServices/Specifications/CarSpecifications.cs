using CarDDD.Core.DomainObjects.DomainCar.EntityObjects;
using CarDDD.Core.DomainObjects.DomainCar.ValueObjects;
using CarDDD.Core.DomainObjects.DomainCar;

namespace CarDDD.DomainServices.Specifications;

/// <summary> Спецификация для создания <see cref="Car"/> </summary>
public record CreateCarSpec
{
    public required string Brand { get; init; }
    public required string Color { get; init; }
    public required decimal Price { get; init; }
    public required Ownership Ownership { get; init; }
    public required Photo Photo { get; init; }
    public required Employer Manager { get; init; }
}

/// <summary>
/// Спецификация для обновления у <see cref="Car"/> бренда, цвета и цены
/// </summary>
public record UpdateBasicCarAttributeSpec
{
    public required string Brand { get; init; }
    public required string Color { get; init; }
    public required decimal Price { get; init; }
}