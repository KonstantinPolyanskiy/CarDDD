using CarDDD.DomainServices.DomainAggregates.CarAggregate;
using CarDDD.DomainServices.DomainAggregates.CarAggregate.Events;
using CarDDD.DomainServices.DomainAggregates.CarAggregate.Results;
using CarDDD.DomainServices.ValueObjects;
using CarDDD.DomainTests.Shared;

namespace CarDDD.DomainTests.Domain.Aggregates.CarTests;

public class CarCreateTests
{
    private readonly ManagerId _manager = ManagerId.From(Guid.NewGuid());
    
    [Fact]
    public void CarCreateWithPhoto_Ok()
    {
        var result = Car.CreateWithPhoto(
            "Lada",
            "Black",
            100_000,
            1_000_000,
            ".jpg",
            _manager);
        
        var events = DomainEventsCollector.GetEvents(result.Car);

        result.Status.Should().Be(CreateCarAction.Success); // успешное создание
        result.Car.Photo.Attached().Should().BeTrue(); // фото прикреплено
        
        events.Should().Contain(e => e is CarCreated); // есть событие о создании
        events.Should().Contain(e => e is CarPhotoAttached); // есть событие о прикрепленном фото
    }

    [Fact]
    public void CarCreateWithoutPhoto_Ok()
    {
        var result = Car.CreateWithoutPhoto(
            "Lada",
            "Black",
            100_000,
            1_000_000,
            _manager);
        
        var events = DomainEventsCollector.GetEvents(result.Car);
        
        result.Status.Should().Be(CreateCarAction.Success);
        result.Car.Photo.Attached().Should().BeFalse();
        result.Car.Photo.Should().Be(Photo.None);
        
        events.Count.Should().Be(1);
        events.Should().Contain(e => e is CarCreated);
    }
    
    [Theory]
    [InlineData("")]
    [InlineData("jpg")]
    public void CarCreateWithPhoto_Error(string extension)
    {
        var result = Car.CreateWithPhoto(
            "Lada",
            "Black",
            100_000,
            1_000_000,
            extension,
            _manager);
        
        var events = DomainEventsCollector.GetEvents(result.Car);

        result.Status.Should().Be(CreateCarAction.ErrorAttachPhoto); 
        result.Car.Photo.Attached().Should().BeFalse();
        result.Car.Photo.Should().Be(Photo.None);
        result.Car.IsAvailable.Should().BeFalse();
        
        events.Count.Should().Be(0);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void CarCreateBadPrice_Error(decimal price)
    {
        var result = Car.CreateWithoutPhoto(
            "Lada",
            "Black",
            price,
            1_000_000,
            _manager);
        
        var events = DomainEventsCollector.GetEvents(result.Car);

        result.Status.Should().Be(CreateCarAction.ErrorInvalidPrice);
        result.Car.IsAvailable.Should().BeFalse();
        
        events.Count.Should().Be(0);
    }
}