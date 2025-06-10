using CarDDD.DomainServices.DomainAggregates.CartAggregate;
using CarDDD.DomainServices.DomainAggregates.CartAggregate.Results;
using CarDDD.DomainServices.ValueObjects;

namespace CarDDD.DomainTests.Domain.Aggregates.CartTests;

public class CartRemoveCarTests
{
    /// <summary>
    /// Нормальный случай удаления машины из корзины
    /// </summary>
    [Fact]
    public void CartRemoveCar_Ok()
    {
        var cart = Cart.Create(CustomerId.From(Guid.NewGuid()));

        var car = new Car(CarId.From(Guid.NewGuid()));

        cart.AddCar(car);

        var result = cart.RemoveCar(car);

        result.Status.Should().Be(CartAction.Success);
        cart.Cars.Count.Should().Be(0);
    } 
        
        
    /// <summary>
    /// Случай удаления машины из пустой корзины
    /// </summary>
    [Fact]
    public void CartTryRemoveFromEmptyCart_Error()
    {
        var cart = Cart.Create(CustomerId.From(Guid.NewGuid()));

        var car = new Car(CarId.From(Guid.NewGuid()));

        var result = cart.RemoveCar(car);

        result.Status.Should().Be(CartAction.ErrorCarNotInCart);
    }
}