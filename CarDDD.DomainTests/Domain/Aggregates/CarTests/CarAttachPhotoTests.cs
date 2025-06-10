using System.Collections.Immutable;
using CarDDD.DomainServices.DomainAggregates.CarAggregate;
using CarDDD.DomainServices.DomainAggregates.CarAggregate.Events;
using CarDDD.DomainServices.DomainAggregates.CarAggregate.Results;
using CarDDD.DomainServices.ValueObjects;
using CarDDD.DomainTests.Shared;

namespace CarDDD.DomainTests.Domain.Aggregates.CarTests;

public class CarAttachPhotoTests
{
    private readonly ManagerId _manager = ManagerId.From(Guid.NewGuid());
    
    /// <summary>
    /// Случай прикрепления фото к машине без фото методом <see cref="Car.AttachPhotoIfNone"/>
    /// </summary>
    [Fact]  
    public void AttachingPhotoIfNone_Ok()
    {
        var result = Car.CreateWithoutPhoto(
            "Lada",
            "Black",
            100_000,
            1_000_000,
            _manager);

        var attachResult = result.Car.AttachPhotoIfNone(".jpg", Employer.From(_manager.Value, ImmutableArray<Role>.Empty));
        
        var events = DomainEventsCollector.GetEvents(result.Car);

        attachResult.Status.Should().Be(UpdateCarAction.Success);

        events.Should().Contain(e => e is CarPhotoAttached);
    }

    /// <summary>
    /// Случай прикрепления фото к машине c уже прикрепленным методом <see cref="Car.AttachPhotoIfNone"/>
    /// </summary>
    [Fact]
    public void CarAttachPhotoIfNoneWherePhotoExist_Error()
    {
        var result = Car.CreateWithPhoto(
            "Lada",
            "Black",
            100_000,
            1_000_000,
            ".jpg",
            _manager);
        
        var attachResult = result.Car.AttachPhotoIfNone(".jpg", Employer.From(_manager.Value, ImmutableArray<Role>.Empty));
        
        var events = DomainEventsCollector.GetEvents(result.Car);
        
        attachResult.Status.Should().Be(UpdateCarAction.ErrorPhotoAlreadyExists);
    }

    /// <summary>
    /// Случай обновления фото машины методом <see cref="Car.AttachOrReplacePhoto"/>
    /// </summary>
    [Fact]
    public void CarUpdatePhoto_Ok()
    {
        var result = Car.CreateWithPhoto(
            "Lada",
            "Black",
            100_000,
            1_000_000,
            ".jpg",
            _manager);

        var firstPhoto = result.Car.Photo;
        
        var updatePhotoResult = result.Car.AttachOrReplacePhoto(".jpg", Employer.From(_manager.Value, ImmutableArray<Role>.Empty));
        
        var events = DomainEventsCollector.GetEvents(result.Car);
        
        updatePhotoResult.Status.Should().Be(UpdateCarAction.Success);
        result.Car.Photo.Attached().Should().BeTrue();
        result.Car.Photo.Should().NotBeEquivalentTo(firstPhoto);
        
        events.Should().HaveCount(3);
        
        events.OfType<CarCreated>()
            .Should().ContainSingle(); 
        
        events.OfType<CarPhotoAttached>()
            .Should().HaveCount(2);  
    }

    /// <summary>
    /// Случай обновления фото машины на невалидное методом <see cref="Car.AttachOrReplacePhoto"/> 
    /// </summary>
    [Fact]
    public void CarUpdatePhoto_Error()
    {
        var result = Car.CreateWithPhoto(
            "Lada",
            "Black",
            100_000,
            1_000_000,
            ".jpg",
            _manager);
        
        var updatePhotoResult = result.Car.AttachOrReplacePhoto("invalid", Employer.From(_manager.Value, ImmutableArray<Role>.Empty));
        
        var events = DomainEventsCollector.GetEvents(result.Car);
        
        updatePhotoResult.Status.Should().Be(UpdateCarAction.ErrorCreatingPhoto);
        events.OfType<CarPhotoAttached>().Should().HaveCount(1);  
    }
}