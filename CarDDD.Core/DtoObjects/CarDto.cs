using CarDDD.Core.DomainObjects.DomainCar;

namespace CarDDD.Core.DtoObjects;

public record AddNewCarDto
{
    public required string Brand { get; init; }
    public required string Color { get; init; }
    public required decimal Price { get; init; }
    
    public required PreviousOwnerDto? PreviousOwner { get; init; }
    
    public required AddCarPhotoDto? Photo { get; init; }
}