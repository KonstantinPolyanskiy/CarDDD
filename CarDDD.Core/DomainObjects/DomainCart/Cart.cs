using CarDDD.Core.DomainObjects.DomainCart.Results;

namespace CarDDD.Core.DomainObjects.DomainCart;

public enum CartAction
{
    Success,
    ErrorAlreadyCheckedOut,
    ErrorCarAlreadyAdded,
    ErrorCarNotInCart
}

public sealed class Cart : AggregateRoot<Guid>
{
    private readonly HashSet<Guid> _carIds = new();
    public IReadOnlyCollection<Guid> CarIds => _carIds;
    public bool IsCheckedOut { get; private set; }
    
    public static Cart Create() => new() { IsCheckedOut = false };
    
    public AddCarToCartResult AddCar(Guid carId)
    {
        if (IsCheckedOut)
            return new(CartAction.ErrorAlreadyCheckedOut);

        if (!_carIds.Add(carId))
            return new(CartAction.ErrorCarAlreadyAdded);

        return new(CartAction.Success);
    }
    
    public RemoveCarFromCartResult RemoveCar(Guid carId)
    {
        if (IsCheckedOut)
            return new(CartAction.ErrorAlreadyCheckedOut);

        if (!_carIds.Remove(carId))
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