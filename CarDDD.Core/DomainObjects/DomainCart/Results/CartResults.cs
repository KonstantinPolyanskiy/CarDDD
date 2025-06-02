using CarDDD.Core.DomainObjects.DomainCart.Actions;

namespace CarDDD.Core.DomainObjects.DomainCart.Results;

public record AddCarToCartResult(CartAction Status);

public record RemoveCarFromCartResult(CartAction Status);

public record PurchaseCartResult(CartAction Status);
