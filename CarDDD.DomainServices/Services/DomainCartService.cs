using CarDDD.DomainServices.DomainAggregates.CarAggregate.Results;
using CarDDD.DomainServices.DomainAggregates.CartAggregate;
using CarDDD.DomainServices.DomainAggregates.CartAggregate.Results;
using CarDDD.DomainServices.Specifications;
using CarDDD.DomainServices.ValueObjects;
using Car = CarDDD.DomainServices.DomainAggregates.CartAggregate.Car;

namespace CarDDD.DomainServices.Services;

/// <summary>
/// <see cref="ICartDomainService"/>
/// </summary>
public class DomainCartService : ICartDomainService
{
    private static readonly IReadOnlyDictionary<CarId, SellCarResult> EmptySellResults = new Dictionary<CarId, SellCarResult>();

    public Cart CreateCart(CreateCartSpec s)
    {
        return Cart.Create(s.Customer);
    }

    public AddCarToCartResult AddCar(Cart cart, AddCarCartSpec s)
    {
        if (!s.Car.IsAvailable)
            return new AddCarToCartResult(CartAction.ErrorCarIsNotAvailable);
        
        return cart.AddCar(new Car(s.Car.CarId));
    }

    public RemoveCarFromCartResult RemoveCar(Cart cart, RemoveCarCartSpec s)
    {
        return cart.RemoveCar(new Car(s.Car.CarId));
    }

    public OrderCartResult OrderCart(Cart cart, OrderCartSpec s)
    {
        // Проверяем что машины в корзине и переданные совпадают
        var matching = MatchCartCarsWithCars(
            s.Cars.Select(c => new Car(c.CarId)).ToList(),
            cart.Cars.ToList()
        );
        if (!matching.match)
            return new OrderCartResult(OrderCartAction.ErrorCarsMismatch);

        // Проверяем что каждая заказываемая машина доступна
        if (s.Cars.Any(x => !x.IsAvailable))
            return new OrderCartResult(OrderCartAction.ErrorSomeCarIsNotAvailable);
        
        return cart.Order();
    }

    public CartPurchaseResult PurchaseCart(Cart cart, PurchaseCartSpec s)
    {
        var carSells = new Dictionary<CarId, SellCarResult>();

        // Проверяем что машины в корзине и переданные совпадают
        var matching = MatchCartCarsWithCars(
            s.CarsToSell.Select(c => new Car(c.CarId)).ToList(),
            cart.Cars.ToList()
        );
        if (!matching.match)
            return matching.outcome;
        
        // Пробуем оплатить корзину
        var purchased = cart.Purchase();
        if (purchased.Status is not PurchaseCartAction.Success)
            return new CartPurchaseResult(purchased, EmptySellResults);

        return new CartPurchaseResult(purchased, carSells);
    }

    public sealed class CartPurchaseResult(
        PurchaseCartResult purchaseResult,
        IReadOnlyDictionary<CarId, SellCarResult> sellResults)
    {
        public PurchaseCartResult PurchaseResult { get; } = purchaseResult ?? throw new ArgumentNullException(nameof(purchaseResult));

        public IReadOnlyDictionary<CarId, SellCarResult> SellResults { get; } = sellResults ?? throw new ArgumentNullException(nameof(sellResults));
    }

    /// <summary>
    /// Проверка, что переданные <see cref="DomainAggregates.CarAggregate.Car"/> в точности совпадают с <see cref="Cart.Cars"/> по идентификатору 
    /// </summary>
    private static (CartPurchaseResult outcome, bool match) MatchCartCarsWithCars(IReadOnlyList<Car> carsToSale, IReadOnlyList<Car> cartCars)
    {
        var domainIds = new HashSet<Guid>(carsToSale.Select(dc => dc.CarId.Value));
        var cartIds = new HashSet<Guid>(cartCars.Select(id => id.CarId.Value));

        if (!domainIds.SetEquals(cartIds))
        {
            return (
                new CartPurchaseResult(
                    new PurchaseCartResult(PurchaseCartAction.ErrorCarsMismatch),
                    EmptySellResults
                ), false
            );
        }
        
        return (null!, true);
    }  
}