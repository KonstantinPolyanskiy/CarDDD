using CarDDD.DomainServices.DomainAggregates.CarAggregate;
using CarDDD.DomainServices.DomainAggregates.CarAggregate.Results;
using CarDDD.DomainServices.DomainAggregates.CartAggregate;
using CarDDD.DomainServices.DomainAggregates.CartAggregate.Results;
using CarDDD.DomainServices.Specifications;
using Car = CarDDD.DomainServices.DomainAggregates.CarAggregate.Car;

namespace CarDDD.DomainServices.Services;


/// <summary>
/// Сервис для централизованного взаимодействия с доменной моделью Машина
/// </summary>
public interface ICarDomainService
{
    public CreateCarResult CreateCar(CreateCarSpec s);
    public UpdateCarResult UpdateCar(Car car, UpdateCarSpec s);
    public SellCarResult SellCar(Car car, SellCarSpec s);
}

/// <summary>
/// Сервис для централизованного взаимодействия с доменной моделью Корзина
/// </summary>
public interface ICartDomainService
{
    public Cart CreateCart(CreateCartSpec s);

    public AddCarToCartResult AddCar(Cart cart, AddCarCartSpec s);
    public RemoveCarFromCartResult RemoveCar(Cart cart, RemoveCarCartSpec s);

    public OrderCartResult OrderCart(Cart cart, OrderCartSpec s);
    public DomainCartService.CartPurchaseResult PurchaseCart(Cart cart, PurchaseCartSpec s);
}