using CarDDD.Core.DomainObjects.DomainCar.Actions;
using CarDDD.Core.DomainObjects.DomainCar.Events;
using CarDDD.Core.DomainObjects.DomainCar.Results;

namespace CarDDD.Core.DomainObjects.DomainCar;

public sealed class Car : AggregateRoot<Guid>
{
    public string Brand { get; private set; } = string.Empty;
    public string Color { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    
    public Condition Condition { get; private set; }
    
    public Ownership PreviousOwner { get; private set; } = Ownership.None;
    public Photo Photo { get; private set; } = Photo.None;
    
    public Guid ResponsiveManagerId { get; private set; }
    
    public bool IsAvailable { get; private set; }

    public static CreateCarResult Create(
        string brand,
        string color,
        decimal price,
        Ownership previousOwner,
        Photo photo,
        Employer manager)
    {
        var car = new Car();
        
        if (car.Price <= 0)
            return new CreateCarResult(CreateCarAction.ErrorInvalidPrice);
        
        car.Brand = brand;
        car.Color = color;
        car.Price = price;
        car.PreviousOwner = previousOwner;
        car.Photo = photo;
        car.ResponsiveManagerId = manager.EntityId;
        
        car.IsAvailable = true;
        
        car.CalculateCondition();

        if (!photo.Attached())
            car.AddDomainEvent(new CreatedWithoutPhoto(car.EntityId, manager.EntityId));

        return new CreateCarResult(CreateCarAction.Success, car);
    }

    public UpdateCarResult Update(string brand, string color, decimal price, Employer manager)
    {
        if (!ItResponsiveManagerOrAdmin(manager))
            return new UpdateCarResult(UpdateCarAction.ErrorEnoughPermission);
        
        Brand = brand;
        Color = color;
        Price = price;
        
        CalculateCondition();
        
        return new UpdateCarResult(UpdateCarAction.Success);
    }

    public UpdateCarResult ChangeManager(Employer newManager, Employer admin)
    {
        if (!ItAdmin(admin))
            return new UpdateCarResult(UpdateCarAction.ErrorEnoughPermission);
        
        ResponsiveManagerId = newManager.EntityId;
        
        return new UpdateCarResult(UpdateCarAction.Success);
    }

    public UpdateCarResult AttachNewPhoto(Photo photo, Employer manager)
    {
        if (!ItResponsiveManagerOrAdmin(manager))
            return new UpdateCarResult(UpdateCarAction.ErrorEnoughPermission);
        
        Photo = photo;
        
        return new UpdateCarResult(UpdateCarAction.Success);
    }

    public SellCarResult Sell(decimal price, Consumer consumer)
    {
        if (price <= 0)
            return new SellCarResult(SellCarAction.ErrorInvalidPrice);
        
        IsAvailable = false;
        
        AddDomainEvent(new CarSold(EntityId, ResponsiveManagerId, consumer.Id));
        
        return new SellCarResult(SellCarAction.Success);
    }

    public static Car Restore(
        Guid id,
        string brand,
        string color,
        decimal price,
        Condition condition,
        Ownership previousOwner,
        Photo photo,
        bool isAvailable,
        Guid responsiveManagerId)
    {
        var car = new Car
        {
            EntityId = id,
            Brand = brand,
            Color = color,
            Price = price,
            Condition = condition,
            PreviousOwner = previousOwner,
            Photo = photo,
            ResponsiveManagerId = responsiveManagerId,
            IsAvailable = isAvailable
        };

        return car;
    }

    private void CalculateCondition()
    {
        if (!PreviousOwner.Exist())
            Condition = Condition.New;
        
        Condition = PreviousOwner.Mileage > 100_000 ? Condition.NotWorking : Condition.Used;
    }

    private bool ItResponsiveManagerOrAdmin(Employer manager)
    {
        if (manager.Roles.Contains(Role.CarAdmin))
            return true;
        
        if (ResponsiveManagerId == manager.EntityId)
            return true;
        
        return false;
    }

    private bool ItAdmin(Employer employer) =>
        employer.Roles.Contains(Role.CarAdmin);
}