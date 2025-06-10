using CarDDD.DomainServices.DomainAggregates.CartAggregate.Results;
using CarDDD.DomainServices.Specifications;
using CarDDD.DomainServices.ValueObjects;

namespace CarDDD.DomainTests.Domain.Services.DomainCartService;

public class AddCarTests
{
    [Fact]
    public void CartService_AddAvailableCar_Ok()
    {
        var service = new DomainServices.Services.DomainCartService();
        var customer = CustomerId.From(Guid.NewGuid());
        var car = new Car
        {
            CarId = CarId.From(Guid.NewGuid()),
            IsAvailable = true
        };

        var cart = service.CreateCart(new CreateCartSpec { Customer = customer });

        var result = service.AddCar(cart, new AddCarCartSpec { Car = car });

        result.Status.Should().Be(CartAction.Success);
    }

    [Fact]
    public void CartService_AddNotAvailableCar_Error()
    {
        var service = new DomainServices.Services.DomainCartService();
        var customer = CustomerId.From(Guid.NewGuid());
        var car = new Car
        {
            CarId = CarId.From(Guid.NewGuid()),
            IsAvailable = false
        };

        var cart = service.CreateCart(new CreateCartSpec { Customer = customer });

        var result = service.AddCar(cart, new AddCarCartSpec { Car = car });

        result.Status.Should().Be(CartAction.ErrorCarIsNotAvailable);
    }
}