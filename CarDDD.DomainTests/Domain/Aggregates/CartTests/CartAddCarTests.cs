using CarDDD.DomainServices.DomainAggregates.CartAggregate;
using CarDDD.DomainServices.DomainAggregates.CartAggregate.Events;
using CarDDD.DomainServices.DomainAggregates.CartAggregate.Results;
using CarDDD.DomainServices.ValueObjects;
using CarDDD.DomainTests.Shared;

namespace CarDDD.DomainTests.Domain.Aggregates.CartTests;

public class CartAddCarTests
{
    [Fact]
    public void CartAddCar_Ok()
    {
        var cart = Cart.Create(CustomerId.From(Guid.NewGuid()));

        var result = cart.AddCar(new Car(CarId.From(Guid.NewGuid())));

        var events = DomainEventsCollector.GetEvents(cart);

        result.Status.Should().Be(CartAction.Success);
        cart.Cars.Should().HaveCount(1);
        events.OfType<AddedCarInCart>().Count().Should().Be(1);
    }

    [Fact]
    public void CartAddCarAndRemoveCar_Ok()
    {
        var cart = Cart.Create(CustomerId.From(Guid.NewGuid()));
        
        var car = new Car(CarId.From(Guid.NewGuid()));
        
        var addResult = cart.AddCar(car);
        var removeResult = cart.RemoveCar(car);
        
        var events = DomainEventsCollector.GetEvents(cart);
        
        addResult.Status.Should().Be(CartAction.Success);
        removeResult.Status.Should().Be(CartAction.Success);
        cart.Cars.Should().HaveCount(0);
        
        events.OfType<AddedCarInCart>().Count().Should().Be(1);
        events.OfType<RemovedCarFromCart>().Count().Should().Be(1);
    }

    [Fact]
    public void TryAddCarInOrderedCart_Error()
    {
        var cart = Cart.Create(CustomerId.From(Guid.NewGuid()));
        
        var firstTryResult = cart.AddCar(new Car(CarId.From(Guid.NewGuid())));

        cart.Order();
        
        var secondTryResult = cart.AddCar(new Car(CarId.From(Guid.NewGuid())));

        firstTryResult.Status.Should().Be(CartAction.Success);
        secondTryResult.Status.Should().Be(CartAction.ErrorCartAlreadyOrdered);
    }

    [Fact]
    public void TryAddCarInPurchasedCart_Error()
    {
        var cart = Cart.Create(CustomerId.From(Guid.NewGuid()));
        
        var firstAddResult = cart.AddCar(new Car(CarId.From(Guid.NewGuid())));
        
        cart.Order();
        cart.Purchase();
        
        var secondTryResult = cart.AddCar(new Car(CarId.From(Guid.NewGuid())));
        
        firstAddResult.Status.Should().Be(CartAction.Success);
        secondTryResult.Status.Should().Be(CartAction.ErrorCartAlreadyOrdered);
    }

    [Fact]
    public void TryAddOneCarTwice_Error()
    {
        var cart = Cart.Create(CustomerId.From(Guid.NewGuid()));
        
        var car  = new Car(CarId.From(Guid.NewGuid()));
        
        var firstAddResult = cart.AddCar(car);
        var secondAddResult = cart.AddCar(car);
        
        firstAddResult.Status.Should().Be(CartAction.Success);
        secondAddResult.Status.Should().Be(CartAction.ErrorCarAlreadyInCart);
    }
}