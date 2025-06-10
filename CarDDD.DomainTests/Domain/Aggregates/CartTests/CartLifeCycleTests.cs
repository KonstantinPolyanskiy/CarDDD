using CarDDD.DomainServices.DomainAggregates.CartAggregate;
using CarDDD.DomainServices.DomainAggregates.CartAggregate.Results;
using CarDDD.DomainServices.ValueObjects;
using CarDDD.DomainTests.Shared;

namespace CarDDD.DomainTests.Domain.Aggregates.CartTests;

public class CartLifeCycleTests
{
    [Fact]
    public void GoodCartLifeCycleTest_Ok()
    {
        var customer = CustomerId.From(Guid.NewGuid());
        
        var carOne = new Car(CarId.From(Guid.NewGuid()));
        var carTwo = new Car(CarId.From(Guid.NewGuid()));
        var carThree = new Car(CarId.From(Guid.NewGuid()));

        var cart = Cart.Create(customer);
        
        cart.AddCar(carOne);
        cart.AddCar(carTwo);
        
        cart.RemoveCar(carTwo);

        cart.Order();
        
        cart.AddCar(carThree);
        
        cart.Purchase();
        
        cart.RemoveCar(carOne);
        
        var events = DomainEventsCollector.GetEvents(cart);

        cart.Ordered.Should().BeTrue();
        cart.Purchased.Should().BeTrue();
        cart.ReadyForPurchase.Should().BeFalse();
        cart.Cars.Should().HaveCount(1);
        
        events.Should().HaveCount(6);
    }
}