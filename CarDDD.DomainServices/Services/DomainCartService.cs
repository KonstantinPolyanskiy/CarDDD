using CarDDD.DomainServices.DomainAggregates.CarAggregate;
using CarDDD.DomainServices.DomainAggregates.CarAggregate.Results;
using CarDDD.DomainServices.DomainAggregates.CartAggregate;
using CarDDD.DomainServices.DomainAggregates.CartAggregate.Results;
using CarDDD.DomainServices.Specifications;
using CarDDD.DomainServices.ValueObjects;

namespace CarDDD.DomainServices.Services;

public class DomainCartService
{
    private static readonly IReadOnlyDictionary<CarId, SellCarResult> EmptySellResults = new Dictionary<CarId, SellCarResult>();

    public Cart CreateCart(CreateCartSpec s)
    {
        return Cart.Create(s.Customer);
    }

    public AddCarToCartResult AddToCart(Cart cart, AddCarCartSpec s)
    {
        return cart.AddCar(s.CarId);
    }

    public RemoveCarFromCartResult RemoveFromCart(Cart cart, RemoveCarCartSpec s)
    {
        return cart.RemoveCar(s.CarId);
    }

    public OrderCartResult Order(Cart cart)
    {
        return cart.Order();
    }

    public CartPurchaseOutcome PurchaseCart(Cart cart, PurchaseCartSpec s)
    {
        var carSells = new Dictionary<CarId, SellCarResult>();

        // Проверяем что машины в корзине и переданные совпадают
        var matching = MatchCartCarsWithSaleCars(s.CarsToSell, cart.Cars.ToList());
        if (!matching.match)
            return matching.outcome;
        
        // Пробуем оплатить корзину
        var purchased = cart.Purchase();
        if (purchased.Status is not PurchaseCartAction.Success)
            return new CartPurchaseOutcome(purchased, EmptySellResults);

        // Корзина успешно оплачена - продаем каждую машину
        foreach (var domainCar in s.CarsToSell)
            carSells[CarId.From(domainCar.EntityId)] = domainCar.Sell(cart.CartOwnerId);
        
        return new CartPurchaseOutcome(purchased, carSells);
    }

    public sealed class CartPurchaseOutcome(
        PurchaseCartResult purchaseResult,
        IReadOnlyDictionary<CarId, SellCarResult> sellResults)
    {
        public PurchaseCartResult PurchaseResult { get; } = purchaseResult ?? throw new ArgumentNullException(nameof(purchaseResult));

        /// <summary>
        /// Результат продажи для каждой DomainAggregates.CarAggregate.Care cref="Car.Sell(Customer)"/>
        /// </summary>
        public IReadOnlyDictionary<CarId, SellCarResult> SellResults { get; } = sellResults ?? throw new ArgumentNullException(nameof(sellResults));
    }

    /// <summary>
    /// Проверка что переданные <see cref="DomainAggregates.CarAggregate.Car"/> в точности совпадают с <see cref="Cart.Cars"/> по идентификатору 
    /// </summary>
    private static (CartPurchaseOutcome outcome, bool match) MatchCartCarsWithSaleCars(IReadOnlyList<Car> carsToSale, IReadOnlyList<CarId> cartCars)
    {
        var domainIds = new HashSet<Guid>(carsToSale.Select(dc => dc.EntityId));
        var cartIds = new HashSet<Guid>(cartCars.Select(id => id.Value));

        if (!domainIds.SetEquals(cartIds))
        {
            return (
                new CartPurchaseOutcome(
                    new PurchaseCartResult(PurchaseCartAction.ErrorCarsMismatch),
                    EmptySellResults
                ), false
            );
        }
        
        return (null!, true);
    }  
}