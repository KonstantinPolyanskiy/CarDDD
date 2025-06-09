using CarDDD.DomainServices.DomainAggregates.CartAggregate;
using CarDDD.DomainServices.ValueObjects;

namespace CarDDD.DomainServices.Specifications;

public record struct Car(CarId CarId, bool IsAvailable);

/// <summary> Спецификация для создания <see cref="Cart"/> </summary>
public record CreateCartSpec
{
    public required CustomerId Customer { get; init; }
}

/// <summary> Спецификация для добавления машины в <see cref="Cart"/> </summary>
public record AddCarCartSpec
{
    public required Car Car { get; init; }
}

/// <summary> Спецификация для удаления машины из <see cref="Cart"/> </summary>
public record RemoveCarCartSpec
{
    public required Car Car { get; init; }
}

/// <summary> Спецификация для заказа машин из <see cref="Cart"/> </summary>
public record OrderCartSpec
{
    public required IReadOnlyList<Car> Cars { get; init; }
}

/// <summary> Спецификация для оплаты корзины и продажи машин <see cref="Car"/> в <see cref="Cart"/> </summary>
public record PurchaseCartSpec
{
    public required IReadOnlyList<Car> CarsToSell { get; init; } 
}