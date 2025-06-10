using System.Collections.Immutable;
using CarDDD.DomainServices.DomainAggregates.CarAggregate.Results;
using CarDDD.DomainServices.Specifications;
using CarDDD.DomainServices.ValueObjects;

namespace CarDDD.DomainTests.Domain.Services.DomainCarService;

public class CreateCarTests
{
    [Fact]
    public void CarService_CreateCarWithoutPhoto_Ok()
    {
        var service = new DomainServices.Services.DomainCarService();

        var spec = new CreateCarSpec
        {
            Brand = "Audi",
            Color = "White",
            Price = 1_000_000,
            Mileage = 0,
            Photo = null,
            Manager = new Employer(EmployerId.From(Guid.NewGuid()), ImmutableArray<Role>.Empty)
        };
        
        var result = service.CreateCar(spec);
        var car = result.Car;

        result.Status.Should().Be(CreateCarAction.Success);
        car.Photo.Attached().Should().BeFalse();
    }

    [Fact]
    public void CarService_CreateCarWithPhoto_Ok()
    {
        var service = new DomainServices.Services.DomainCarService();
        
        var spec = new CreateCarSpec
        {
            Brand = "Audi",
            Color = "White",
            Price = 1_000_000,
            Mileage = 0,
            Photo = new AttachCarPhotoSpec
            {
                Extension = ".jpg",
                OnlyIfNonePhoto = true
            },
            Manager = new Employer(EmployerId.From(Guid.NewGuid()), ImmutableArray<Role>.Empty)
        };
        
        var result = service.CreateCar(spec);
        var car = result.Car;
        
        result.Status.Should().Be(CreateCarAction.Success);
        car.Photo.Attached().Should().BeTrue();
        car.Photo.Extension.Should().Be(".jpg");
    }

    /// <summary>
    /// В <see cref="AttachCarPhotoSpec"/> есть поле OnlyIfNonePhoto.
    /// В случае создания машины мы должны просто игнорировать данное поле.
    /// </summary>
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void CarService_CreateCarWithPhotoOnlyIfNonePhoto_Ok(bool onlyIfNonePhoto)
    {
        var service = new DomainServices.Services.DomainCarService();

        var spec = new CreateCarSpec
        {
            Brand = "Audi",
            Color = "White",
            Price = 1_000_000,
            Mileage = 0,
            Photo = new AttachCarPhotoSpec
            {
                Extension = ".jpg",
                OnlyIfNonePhoto = onlyIfNonePhoto
            },
            Manager = new Employer(EmployerId.From(Guid.NewGuid()), ImmutableArray<Role>.Empty)
        };
        
        var result = service.CreateCar(spec);
        var car = result.Car;
        
        result.Status.Should().Be(CreateCarAction.Success);
        car.Photo.Attached().Should().BeTrue();
        car.Photo.Extension.Should().Be(".jpg");
    }
}