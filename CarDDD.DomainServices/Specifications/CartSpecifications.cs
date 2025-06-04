using CarDDD.DomainServices.DomainAggregates.CarAggregate;
using CarDDD.DomainServices.DomainAggregates.CartAggregate;
using CarDDD.DomainServices.ValueObjects;

namespace CarDDD.DomainServices.Specifications;

/// <summary> Спецификация для создания <see cref="Cart"/> </summary>
public record CreateCartSpec
{
    public required CustomerId Customer { get; init; }
}

/// <summary> Спецификация для добавления машины в <see cref="Cart"/> </summary>
public record AddCarCartSpec
{
    public required CarId CarId { get; init; }
}

/// <summary> Спецификация для удаления машины из <see cref="Cart"/> </summary>
public record RemoveCarCartSpec
{
    public required CarId CarId { get; init; }
}

/// <summary> Спецификация для оплаты корзины и продажи машин <see cref="Car"/> в <see cref="Cart"/> </summary>
public record PurchaseCartSpec
{
    public required IReadOnlyList<Car> CarsToSell { get; init; } 
}