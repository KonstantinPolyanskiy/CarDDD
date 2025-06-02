namespace CarDDD.Core.DtoObjects;

public record AddCarPhotoDto
{
    public required string Extension { get; init; }
    public required byte[] Data { get; init; }
}