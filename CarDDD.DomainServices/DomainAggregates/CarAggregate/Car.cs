using CarDDD.DomainServices.DomainAggregates.CarAggregate.Events;
using CarDDD.DomainServices.DomainAggregates.CarAggregate.Results;
using CarDDD.DomainServices.ValueObjects;

namespace CarDDD.DomainServices.DomainAggregates.CarAggregate;


/// <summary>
/// Агрегат Машина
/// </summary>
public sealed class Car : AggregateRoot<Guid>
{
    #region Fields

    public string Brand { get; private set; } = string.Empty;
    public string Color { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public int Mileage { get; private set; }
    
    /// <summary> Состояние машины </summary>
    public Condition Condition { get; private set; }
    public Photo Photo { get; private set; } = Photo.None;
    
    public ManagerId ManagerId { get; private set; } 

    public bool IsAvailable { get; private set; }

    #endregion
    
    /// <summary>
    /// Создать машину с фото
    /// </summary>
    public static CreateCarResult CreateWithPhoto(string brand, string color, decimal price, int mileage, string photoExtension, ManagerId managerId)
    {
        var created = CreateBase(brand, color, price, mileage, managerId);
        if (created.Status is not CreateCarAction.Success)
            return created;
        var car = created.Car;
        
        car.Photo = Photo.CreateNew(photoExtension);
        if (!car.Photo.Attached())
            return new CreateCarResult(CreateCarAction.ErrorAttachPhoto, EmptyCar);
        
        car.IsAvailable = true;

        car.AddDomainEvent(
            new CarPhotoAttached(
                CarId.From(car.EntityId),
                PhotoId.From(car.Photo.Id),
                EmployerId.From(managerId.Value)
            )
        );
        
        car.AddDomainEvent(
            new CarCreated(
                CarId.From(car.EntityId),
                EmployerId.From(managerId.Value),
                false
            )
        );

        return new CreateCarResult(CreateCarAction.Success, car);
    }
    
    /// <summary>
    /// Создать машину без фото
    /// </summary>
    public static CreateCarResult CreateWithoutPhoto(string brand, string color, decimal price, int mileage, ManagerId managerId)
    {
        var created = CreateBase(brand, color, price, mileage, managerId);
        if (created.Status is not CreateCarAction.Success) 
            return created;
        var car = created.Car;

        car.Photo = Photo.None;
        
        car.IsAvailable = true;

        car.AddDomainEvent(
            new CarCreated(
                CarId.From(car.EntityId),
                EmployerId.From(managerId.Value),
                false
            )
        );
        
        return created;
    }

    private static CreateCarResult CreateBase(string brand, string color, decimal price, int mileage, ManagerId managerId)
    {
        var car = new Car();
        
        if (price <= 0)
            return new CreateCarResult(CreateCarAction.ErrorInvalidPrice, EmptyCar);
        
        car.Brand = brand;
        car.Color = color;
        car.Price = price;
        car.Mileage = mileage;
        car.ManagerId = managerId;
        
        car.CalculateCondition(); 
            
        return new CreateCarResult(CreateCarAction.Success, car);
    }
    
    /// <summary> Обновить стандартные поля машины - бренд, цвет, цену и пробег </summary>
    public UpdateCarResult Update(string brand, string color, decimal price, int mileage, Employer employer)
    {
        // Изменить может администратор и ответственный
        if (!ResponsiveManagerOrAdmin(employer))
            return new UpdateCarResult(UpdateCarAction.ErrorEnoughPermission);

        if (price <= 0)
            return new UpdateCarResult(UpdateCarAction.ErrorInvalidPrice);
        
        Brand = brand;
        Color = color;
        Price = price;
        Mileage = mileage;
        
        CalculateCondition();
        
        AddDomainEvent(new CarUpdatedBasisAttributes(CarId.From(EntityId), employer.Id));
        
        return new UpdateCarResult(UpdateCarAction.Success);
    }
    
    /// <summary> Сменить ответственного менеджера у машины </summary>
    public UpdateCarResult ChangeManager(ManagerId newManagerId, Employer caller)
    {
        // Изменить может только администратор
        if (NotAdmin(caller))
            return new UpdateCarResult(UpdateCarAction.ErrorEnoughPermission);

        // Сохраняем id старого менеджера
        ManagerId previousManagerId = ManagerId;
        
        // Назначаем нового менеджера
        ManagerId = newManagerId;

        AddDomainEvent(
            new CarManagerChanged(
                CarId.From(EntityId),
                EmployerId.From(newManagerId.Value),
                EmployerId.From(previousManagerId.Value),
                caller.Id
            )
        );
        
        return new UpdateCarResult(UpdateCarAction.Success);
    }
    
    /// <summary> Прикрепить фото к машине </summary>
    public UpdateCarResult AttachOrReplacePhoto(string extension, Employer caller)
    {
        if (!ResponsiveManagerOrAdmin(caller))
            return new UpdateCarResult(UpdateCarAction.ErrorEnoughPermission);
        
        var p = Photo.CreateNew(extension);
        if (p == Photo.None)
            return new UpdateCarResult(UpdateCarAction.ErrorCreatingPhoto);

        Photo = p;
        if (!Photo.Attached())
            return new UpdateCarResult(UpdateCarAction.ErrorPhotoNotAttached);

        AddDomainEvent(
            new CarPhotoAttached(
                CarId.From(EntityId),
                PhotoId.From(Photo.Id),
                EmployerId.From(caller.Id.Value)
            )
        );
        
        return new UpdateCarResult(UpdateCarAction.Success);
    }

    public UpdateCarResult AttachPhotoIfNone(string extension, Employer caller)
    {
        if (!ResponsiveManagerOrAdmin(caller))
            return new UpdateCarResult(UpdateCarAction.ErrorEnoughPermission);
        
        if (Photo.Attached())
            return new UpdateCarResult(UpdateCarAction.ErrorPhotoAlreadyExists);
        
        var p = Photo.CreateNew(extension);
        if (p == Photo.None)
            return new UpdateCarResult(UpdateCarAction.ErrorCreatingPhoto);
        
        Photo = p;
        if (!Photo.Attached())
            return new UpdateCarResult(UpdateCarAction.ErrorPhotoNotAttached);
        
        AddDomainEvent(
            new CarPhotoAttached(
                CarId.From(EntityId),
                PhotoId.From(Photo.Id),
                EmployerId.From(caller.Id.Value)
            )
        );
        
        return new UpdateCarResult(UpdateCarAction.Success);
    }
    
    /// <summary> Продать машину </summary>
    public SellCarResult Sell(CustomerId customerId)
    {
        if (!IsAvailable)
            return new SellCarResult(SellCarAction.ErrorIsNotAvailable);
        
        IsAvailable = false;

        AddDomainEvent(
            new CarSold(
                CarId.From(EntityId),
                EmployerId.From(ManagerId.Value),
                CustomerId.From(customerId.Value)
            )
        );
        
        return new SellCarResult(SellCarAction.Success);
    }

    public static Car Restore(
        CarId id,
        string brand,
        string color,
        decimal price,
        int mileage,
        Condition condition,
        PhotoId photoId,
        string photoExtension,
        bool isAvailable,
        EmployerId responsiveManagerId)
    {
        var car = new Car
        {
            EntityId = id.Value,
            Brand = brand,
            Color = color,
            Price = price,
            Mileage = mileage,
            Condition = condition,
            Photo = photoId.Value == Guid.Empty 
                ? Photo.None 
                : new Photo(photoId.Value, photoExtension),
            ManagerId = ManagerId.From(responsiveManagerId.Value),
            IsAvailable = isAvailable
        };

        return car;
    }

    private void CalculateCondition()
    {
        if (Mileage == 0)
        {
            Condition = Condition.New;
            return;
        }
        
        Condition = Mileage > 100_000 ? Condition.NotWorking : Condition.Used;
    }

    private bool ResponsiveManagerOrAdmin(Employer caller)
    {
        if (caller.Roles.Contains(Role.CarAdmin))
            return true;
        
        if (caller.Id.Value == ManagerId.Value)
            return true;
        
        return false;
    }
    
    private bool NotAdmin(Employer employer) => 
        !employer.Roles.Contains(Role.CarAdmin);
    
    private static Car EmptyCar => new Car();
}