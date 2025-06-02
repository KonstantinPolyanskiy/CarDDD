using CarDDD.Core.DomainObjects.DomainCart.Results;

namespace CarDDD.Core.DomainObjects.DomainCart;

public enum CartAction
{
    Success,
    ErrorAlreadyCheckedOut,
    ErrorCarAlreadyAdded,
    ErrorCarNotInCart
}

public record Car(Guid CarId);

public sealed class Cart : AggregateRoot<Guid>
{
    private readonly HashSet<Car> cars = new();
    public IReadOnlyCollection<Car> Cars => cars;
    public bool IsCheckedOut { get; private set; }
    
    public static Cart Create() => new() { IsCheckedOut = false };
    
    public AddCarToCartResult AddCar(Car car)
    {
        if (IsCheckedOut)
            return new(CartAction.ErrorAlreadyCheckedOut);

        if (!cars.Add(car))
            return new(CartAction.ErrorCarAlreadyAdded);

        return new(CartAction.Success);
    }
    
    public RemoveCarFromCartResult RemoveCar(Car car)
    {
        if (IsCheckedOut)
            return new(CartAction.ErrorAlreadyCheckedOut);

        if (!cars.Remove(car))
            return new(CartAction.ErrorCarNotInCart);

        return new(CartAction.Success);
    }
    
    public PurchaseCartResult MarkPurchased()
    {
        if (IsCheckedOut)
            return new(CartAction.ErrorAlreadyCheckedOut);

        IsCheckedOut = true;
        return new(CartAction.Success);
    }
}