using System.Collections.Immutable;
using System.Drawing;
using CarDDD.Core.DomainObjects;
using CarDDD.Core.DomainObjects.DomainCar;
using CarDDD.Core.DomainObjects.DomainCar.ValueObjects;

namespace CarDDD.Core.AnswerObjects.ServiceResponses;

public record CarInfo
{
    public CarInfo() {}
    public CarInfo(Car car)
    {
        Id = car.EntityId;
        
        Brand = car.Brand;
        Color = car.Color;
        Price = car.Price;
        Condition = car.Condition;
        IsAvailable = car.IsAvailable;
        PreviousOwnerName = car.PreviousOwner.Name;
        Mileage = car.PreviousOwner.Mileage;
    }
    public Guid? Id { get; set; }
    
    public string? Brand { get; set; } 
    public string? Color { get; set; } 
    public decimal? Price { get; set; } 
    
    public Condition? Condition { get; set; }
    public bool? IsAvailable { get; set; }
    
    public string? PreviousOwnerName { get; set; }
    public int? Mileage { get; set; }
    
    public UserInfo? Manager { get; set; }
    
    public UserInfo? Purchaser { get; set; }
    
    public PhotoInfo? Photo { get; set; }
}

public record UserInfo
{
    public Guid? Id { get; set; }
    
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    
    public string? Email { get; set; }
    public string? Login  { get; set; }

    public ImmutableArray<Role>? Roles { get; set; } = [];
}

public record PhotoInfo
{
    public Guid? Id { get; set; }
    
    public string? Extension { get; set; }
    public ImmutableArray<byte>? Data { get; set; }
}