using CarDDD.DomainServices.DomainAggregates.CartAggregate.Events;
using CarDDD.DomainServices.DomainAggregates.CartAggregate.Results;
using CarDDD.DomainServices.ValueObjects;

namespace CarDDD.DomainServices.DomainAggregates.CartAggregate;


/// <summary> Доменная модель корзины </summary>
public sealed class Cart : AggregateRoot<Guid>
{
    #region Fields
    
    private readonly HashSet<CarId> _cars = new();
    public IReadOnlyCollection<CarId> Cars => _cars;

    /// <summary> Клиент, к которому привязана корзина </summary>
    public CustomerId CartOwnerId { get; private init; } 
    
    /// <summary> Заказана ли корзина </summary>
    public bool Ordered { get; private set; }
    
    /// <summary> Готова ли корзина к покупке </summary>
    public bool ReadyForPurchase { get; private set; }
    
    /// <summary> Оплачена ли корзина </summary>
    public bool Purchased { get; private set; }
    
    #endregion

    /// <summary> Создать пустую корзину с владельцем </summary>
    public static Cart Create(CustomerId customerId)
    {
        var cart = new Cart
        {
            CartOwnerId = customerId,
            
            Ordered = false,
            ReadyForPurchase = false,
            Purchased = false
        };

        cart.AddDomainEvent(new CartCreated(CartId.From(cart.EntityId), customerId));
        
        return cart;
    }
    
    /// <summary> Добавить машину в корзину </summary>
    public AddCarToCartResult AddCar(CarId carId)
    {
        if (Ordered)
            return new AddCarToCartResult(CartAction.ErrorCartAlreadyOrdered);
        
        if (!_cars.Add(carId))
            return new(CartAction.ErrorCarAlreadyInCart);

        AddDomainEvent(
            new AddedCarInCart(
                CartId.From(EntityId),
                CartOwnerId,
                carId
            )
        );

        return new AddCarToCartResult(CartAction.Success);
    }
    
    /// <summary> Удалить машину в корзину </summary>
    public RemoveCarFromCartResult RemoveCar(CarId carId)
    {
        if (Ordered)
            return new RemoveCarFromCartResult(CartAction.ErrorCartAlreadyOrdered);

        if (!_cars.Remove(carId))
            return new RemoveCarFromCartResult(CartAction.ErrorCarNotInCart);

        AddDomainEvent(
            new RemovedCarFromCart(
                CartId.From(EntityId),
                CartOwnerId,
                carId
            )
        );

        return new RemoveCarFromCartResult(CartAction.Success);
    }

    /// <summary> Заказать корзину </summary>
    public OrderCartResult Order()
    {
        if (Ordered)
            return new OrderCartResult(OrderCartAction.ErrorAlreadyOrdered);

        if (!_cars.Any())
            return new OrderCartResult(OrderCartAction.ErrorCartEmpty);
        
        // Корзина заказана
        Ordered = true;
        
        // Корзина готовка в оплате
        ReadyForPurchase = true;

        AddDomainEvent(
            new CartOrdered(
                CartId.From(EntityId),
                OrderedCars,
                CartOwnerId
            )
        );
        
        return new OrderCartResult(OrderCartAction.Success);
    }

    /// <summary> Оплатить корзину (только после заказа) </summary>
    public PurchaseCartResult Purchase()
    {
        if (!Ordered)
            return new PurchaseCartResult(PurchaseCartAction.ErrorCartNotOrdered);

        if (!ReadyForPurchase)
            return new PurchaseCartResult(PurchaseCartAction.ErrorNotReadyForPurchase);
        
        if (Purchased)
            return new PurchaseCartResult(PurchaseCartAction.ErrorAlreadyPurchased);
        
        // Корзина оплачена
        Purchased = true;
        
        // Корзину нельзя повторно оплатить
        ReadyForPurchase = false;

        AddDomainEvent(
            new CartPurchased(
                CartId.From(EntityId),
                PurchasedCars,
                CartOwnerId
            )
        );
        
        return new PurchaseCartResult(PurchaseCartAction.Success);
    }
    
    public static Cart Restore(
        Guid id,
        IEnumerable<CarId> cars,
        CustomerId cartOwnerId,
        bool ordered,
        bool readyForPurchase,
        bool purchased)
    {
        var cart = new Cart
        {
            EntityId = id,
            CartOwnerId = cartOwnerId,
            Ordered = ordered,
            ReadyForPurchase = readyForPurchase,
            Purchased = purchased,
        };

        foreach (var carId in cars)
        {
            cart._cars.Add(carId);
        }

        return cart;
    }
    
    private IReadOnlyList<CarId> OrderedCars => _cars.Select(car => car).ToList();
    private IReadOnlyList<CarId> PurchasedCars => OrderedCars;
}