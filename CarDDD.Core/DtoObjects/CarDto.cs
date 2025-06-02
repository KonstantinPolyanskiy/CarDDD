using CarDDD.Core.DomainObjects.DomainCar;
using CarDDD.Core.DomainObjects.DomainCar.ValueObjects;

namespace CarDDD.Core.DtoObjects;

public record AddNewCarDto
{
    public required string Brand { get; init; }
    public required string Color { get; init; }
    public required decimal Price { get; init; }
    
    public required PreviousOwnerDto? PreviousOwner { get; init; }
    
    public required AddCarPhotoDto? Photo { get; init; }
}

public record UpdateCarDto
{
    public required string Brand { get; init; }
    public required string Color { get; init; }
    public required decimal Price { get; init; }
}

public record SearchCarDto
{
    public string[]? Brands { get; init; }
    public string[]? Colors { get; init; }
    
    public bool OnlyAvailable { get; init; }
    
    public Condition? Condition { get; init; } 
    
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
