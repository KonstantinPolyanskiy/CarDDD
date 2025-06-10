using CarDDD.DomainServices.DomainAggregates.CartAggregate.Results;
using CarDDD.DomainServices.Specifications;
using CarDDD.DomainServices.ValueObjects;

namespace CarDDD.DomainTests.Domain.Services.DomainCartService;

public class OrderCartTests
{
    [Fact]
    public void CartService_OrderCart_WithAvailableCars_Ok()
    {
        var service = new DomainServices.Services.DomainCartService();
        var customer = CustomerId.From(Guid.NewGuid());

        var cart = service.CreateCart(new CreateCartSpec { Customer = customer });
        
        var carIds = new [] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() }
            .Select(CarId.From)
            .ToList();

        var cars = carIds
            .Select(id => new Car(id, true))
            .ToList();
        
        foreach (var car in cars)
            service.AddCar(cart, new AddCarCartSpec { Car = car });
        
        var orderResult = service.OrderCart(cart, new OrderCartSpec { Cars = cars});

        orderResult.Status.Should().Be(OrderCartAction.Success);
        cart.Cars.Count.Should().Be(cars.Count);
    }

    [Fact]
    public void CartService_OrderCart_WithOneUnavailableCars_Error()
    {
        var service = new DomainServices.Services.DomainCartService();
        var customer = CustomerId.From(Guid.NewGuid());
        
        var cart = service.CreateCart(new CreateCartSpec { Customer = customer });
        
        var carIds = new [] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() }
            .Select(CarId.From)
            .ToList();
        
        var cars = carIds
            .Select(id => new Car(id, true))
            .ToList();

        foreach (var car in cars)
            service.AddCar(cart, new AddCarCartSpec { Car = car });

        var orderingCars = cars;
        orderingCars[0] = cars[0] with {IsAvailable = false };
        
        var orderResult = service.OrderCart(cart, new OrderCartSpec { Cars = orderingCars });

        orderResult.Status.Should().Be(OrderCartAction.ErrorSomeCarIsNotAvailable);
        cart.Cars.Count.Should().Be(cars.Count);
        cart.Ordered.Should().BeFalse();
    }
}