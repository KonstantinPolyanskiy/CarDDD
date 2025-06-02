namespace CarDDD.Core.DtoObjects;

public record PreviousOwnerDto
{
    public required string Name { get; init; }
    public required int Mileage { get; init; }
}