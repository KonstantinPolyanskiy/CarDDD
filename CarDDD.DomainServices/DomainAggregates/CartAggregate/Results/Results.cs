using CarDDD.DomainServices.DomainAggregates.CarAggregate;

namespace CarDDD.DomainServices.DomainAggregates.CartAggregate.Results;

/// <summary> Результат добавления ссылки на <see cref="Car"/> в <see cref="Cart"/> </summary>
public record AddCarToCartResult(CartAction Status);

/// <summary> Результат удаления ссылки на <see cref="Car"/> в <see cref="Cart"/> </summary>
public record RemoveCarFromCartResult(CartAction Status);

/// <summary> Результат заказа добавленных в <see cref="Cart"/> набора <see cref="Car"/> </summary>
public record OrderCartResult(OrderCartAction Status);

/// <summary> Результат оплаты заказа добавленных в <see cref="Cart"/> набора <see cref="Car"/> </summary>
public record PurchaseCartResult(PurchaseCartAction Status);

