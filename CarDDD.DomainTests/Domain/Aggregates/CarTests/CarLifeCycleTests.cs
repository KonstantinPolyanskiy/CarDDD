using System.Collections.Immutable;
using CarDDD.DomainServices.DomainAggregates.CarAggregate;
using CarDDD.DomainServices.DomainAggregates.CarAggregate.Events;
using CarDDD.DomainServices.DomainAggregates.CarAggregate.Results;
using CarDDD.DomainServices.ValueObjects;
using CarDDD.DomainTests.Shared;

namespace CarDDD.DomainTests.Domain.Aggregates.CarTests;

public class CarLifeCycleTests
{
    [Fact]
    public void GoodCarLifeCycleTest_Ok()
    {
        var originalManager = ManagerId.From(Guid.NewGuid());
        var newManager = ManagerId.From(Guid.NewGuid());
        var admin = Employer.From(Guid.NewGuid(), [Role.CarAdmin]);
        
        var createResult = Car.CreateWithoutPhoto(
            "Lada",
            "Black",
            100_000,
            1_000_000,
            originalManager);
        var car = createResult.Car;

        car.AttachPhotoIfNone(".jpg", Employer.From(originalManager.Value, ImmutableArray<Role>.Empty));

        car.Update(
            createResult.Car.Brand,
            createResult.Car.Color,
            200_000,
            createResult.Car.Mileage,
            Employer.From(originalManager.Value, ImmutableArray<Role>.Empty)
        );
        
        car.ChangeManager(ManagerId.From(newManager.Value), admin);
        
        car.AttachOrReplacePhoto(".png", Employer.From(newManager.Value, ImmutableArray<Role>.Empty));
        
        car.Sell(CustomerId.From(Guid.NewGuid()));
        
        var events = DomainEventsCollector.GetEvents(createResult.Car);

        createResult.Status.Should().Be(CreateCarAction.Success);
        car.IsAvailable.Should().BeFalse();

        car.Photo.Attached().Should().BeTrue();
        car.Photo.Extension.Should().Be(".png");
        
        car.Price.Should().Be(200_000);

        events.Should().HaveCount(6);
        
        events.OfType<CarCreated>().Count().Should().Be(1);
        events.OfType<CarPhotoAttached>().Count().Should().Be(2);
        events.OfType<CarManagerChanged>().Count().Should().Be(1);
        events.OfType<CarUpdatedBasisAttributes>().Count().Should().Be(1);
        events.OfType<CarSold>().Count().Should().Be(1);
    }
}