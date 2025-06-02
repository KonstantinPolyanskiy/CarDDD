using System.Collections.Immutable;
using CarDDD.Core.DomainObjects.CommonValueObjects;
using CarDDD.Core.DomainObjects.DomainCart.Actions;
using CarDDD.Core.DomainObjects.DomainCart.Events;
using CarDDD.Core.DomainObjects.DomainCart.Results;
using CarDDD.Core.EntityObjects;

namespace CarDDD.Core.DomainObjects.DomainCart;

public record Car(CarId CarId, bool IsAvailable = false);


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
        
        if (!car.IsAvailable)
            return new(CartAction.ErrorCarNotAvailable);

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
    
    public PurchaseCartResult MarkPurchased(ConsumerId purchaserId)
    {
        if (IsCheckedOut)
            return new(CartAction.ErrorAlreadyCheckedOut);
        
        AddDomainEvent(new CartOrdered(PurchasedCarIds, purchaserId));

        IsCheckedOut = true;
        return new(CartAction.Success);
    }
    
    private IReadOnlyList<CarId> PurchasedCarIds => Cars.Select(x => x.CarId).ToList();
}