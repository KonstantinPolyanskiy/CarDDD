using CarDDD.DomainServices.DomainAggregates.CarAggregate;
using CarDDD.DomainServices.DomainAggregates.CarAggregate.Events;
using CarDDD.DomainServices.DomainAggregates.CarAggregate.Results;
using CarDDD.DomainServices.ValueObjects;
using CarDDD.DomainTests.Shared;

namespace CarDDD.DomainTests.Domain.Aggregates.CarTests;

public class CarSellTests
{
    private readonly ManagerId _manager = ManagerId.From(Guid.NewGuid());
    
    [Fact]
    public void CarSell_Ok()
    {
        var result = Car.CreateWithPhoto(
            "Lada",
            "Black",
            100_000,
            1_000_000,
            ".jpg",
            _manager);
        
        var customer = CustomerId.From(Guid.NewGuid());

        var sellResult = result.Car.Sell(customer);

        var events = DomainEventsCollector.GetEvents(result.Car);

        sellResult.Status.Should().Be(SellCarAction.Success);
        result.Car.IsAvailable.Should().BeFalse();
        events.Should().HaveCount(3);

        var sellEvent = events.First(e => e is CarSold) as CarSold;
        
        sellEvent!.CarId.Value.Should().Be(result.Car.EntityId);
        sellEvent.CustomerId.Should().Be(customer);
    }

    [Fact]
    public void CarSellTryTwice_Error()
    {
        var result = Car.CreateWithPhoto(
            "Lada",
            "Black",
            100_000,
            1_000_000,
            ".jpg",
            _manager);
        
        var customer = CustomerId.From(Guid.NewGuid());

        var firstSell = result.Car.Sell(customer);
        var secondSell = result.Car.Sell(customer);
        
        var events = DomainEventsCollector.GetEvents(result.Car);
        
        events.Should().HaveCount(3);
        firstSell.Status.Should().Be(SellCarAction.Success);
        secondSell.Status.Should().Be(SellCarAction.ErrorIsNotAvailable);

    }
}