using System.Collections.Immutable;
using CarDDD.DomainServices.DomainAggregates.CarAggregate.Results;
using CarDDD.DomainServices.Specifications;
using CarDDD.DomainServices.ValueObjects;

namespace CarDDD.DomainTests.Domain.Services.DomainCarService;

public class UpdateCarTests
{
    [Fact]
    public void CarService_UpdateCar_AssignManager_Ok()
    {
        var service = new DomainServices.Services.DomainCarService();
        
        var originalManager = Employer.From(Guid.NewGuid(), ImmutableArray<Role>.Empty);
        var assignedManager = Employer.From(Guid.NewGuid(), ImmutableArray<Role>.Empty);
        var admin = Employer.From(Guid.NewGuid(), [Role.CarAdmin]);

        var createCarSpec = new CreateCarSpec
        {
            Brand = "Audi",
            Color = "White",
            Price = 1_000_000,
            Mileage = 0,
            Photo = null,
            Manager = originalManager
        };
        
        var createResult = service.CreateCar(createCarSpec);
        var car = createResult.Car;

        var assigningResult = service.UpdateCar(car, new UpdateCarSpec
        {
            BasisAttributesSpec = null,
            PhotoSpec = null,
            AssignManagerSpec = new AssignCarManagerSpec { AssignmentManager = assignedManager },
            Employer = admin
        });

        assigningResult.Status.Should().Be(UpdateCarAction.Success);
        car.ManagerId.Should().Be(ManagerId.From(assignedManager.Id.Value));
    }

    [Fact]
    public void CarService_UpdateCar_FullSpecification_Ok()
    {
        var service = new DomainServices.Services.DomainCarService();
        
        var originalManager = Employer.From(Guid.NewGuid(), ImmutableArray<Role>.Empty);
        var assignedManager = Employer.From(Guid.NewGuid(), ImmutableArray<Role>.Empty);
        var admin = Employer.From(Guid.NewGuid(), [Role.CarAdmin]);

        var createResult = service.CreateCar(new CreateCarSpec
        {
            Brand = "Audi",
            Color = "White",
            Price = 1_000_000,
            Mileage = 0,
            Photo = new AttachCarPhotoSpec
            {
                Extension = ".jpg",
                OnlyIfNonePhoto = false
            },
            Manager = originalManager
        });
        var car = createResult.Car;

        var updateResult = service.UpdateCar(car, new UpdateCarSpec
        {
            BasisAttributesSpec = new UpdateCarBasicAttributesSpec
            {
                Brand = "Lada",
                Color = "Black",
                Price = 99,
                Mileage = 100_000
            },
            PhotoSpec = new AttachCarPhotoSpec
            {
                Extension = ".png",
                OnlyIfNonePhoto = false
            },
            AssignManagerSpec = new AssignCarManagerSpec
            {
                AssignmentManager = assignedManager,
            },
            Employer = admin
        });
        
        updateResult.Status.Should().Be(UpdateCarAction.Success);
        
        car.ManagerId.Should().Be(ManagerId.From(assignedManager.Id.Value));
        car.Brand.Should().Be("Lada");
        car.Color.Should().Be("Black");
        car.Price.Should().Be(99);
        car.Mileage.Should().Be(100_000);
        car.Photo.Extension.Should().Be(".png");
    }

    [Fact]
    public void CarService_UpdateCar_BasicAttributesSpecification_NotResponsiveManager_Error()
    {
        var service = new DomainServices.Services.DomainCarService();
        
        var responsiveManager = Employer.From(Guid.NewGuid(), ImmutableArray<Role>.Empty);
        var otherManager = Employer.From(Guid.NewGuid(), ImmutableArray<Role>.Empty);
        
        var createResult = service.CreateCar(new CreateCarSpec
        {
            Brand = "Audi",
            Color = "White",
            Price = 1_000_000,
            Mileage = 0,
            Photo = new AttachCarPhotoSpec
            {
                Extension = ".jpg",
                OnlyIfNonePhoto = false
            },
            Manager = responsiveManager
        });
        var car = createResult.Car;

        var updateBasicResult = service.UpdateCar(car, new UpdateCarSpec
        {
            BasisAttributesSpec = new UpdateCarBasicAttributesSpec
            {
                Brand = "Lada",
                Color = "Black",
                Price = 99,
                Mileage = 100_000
            },
            PhotoSpec = null,
            AssignManagerSpec = null,
            Employer = otherManager
        });

        updateBasicResult.Status.Should().Be(UpdateCarAction.ErrorEnoughPermission);
        
        car.Brand.Should().Be("Audi");
        car.Color.Should().Be("White");
        car.Price.Should().Be(1_000_000);
        car.Mileage.Should().Be(0);
    }
}