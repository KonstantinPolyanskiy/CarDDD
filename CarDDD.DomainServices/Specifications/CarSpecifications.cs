using CarDDD.DomainServices.DomainAggregates.CarAggregate;
using CarDDD.DomainServices.ValueObjects;

namespace CarDDD.DomainServices.Specifications;

/// <summary> Спецификация для создания <see cref="Car"/> </summary>
public record CreateCarSpec
{
    public required string Brand { get; init; }
    public required string Color { get; init; }
    public required decimal Price { get; init; }
    
    public required int Mileage { get; init; }
    
    public required AttachCarPhotoSpec? Photo { get; init; }
    public required Employer Manager { get; init; }
}

/// <summary> Спецификация для продажи <see cref="Car"/> покупателю </summary>
public record SellCarSpec
{
    public required CustomerId CustomerId { get; init; }
}

/// <summary> Спецификация для обновления у <see cref="Car"/> общего типа </summary>
public record UpdateCarSpec
{
    public UpdateCarBasicAttributesSpec? BasisAttributesSpec { get; init; }
    
    public AttachCarPhotoSpec? PhotoSpec { get; init; }
    
    public AssignCarManagerSpec? AssignManagerSpec { get; init; }
    
    
    public required Employer Employer { get; init; }
}

/// <summary> Спецификация для обновления у <see cref="Car"/> бренда, цвета и цены </summary>
public record UpdateCarBasicAttributesSpec
{
    public required string Brand { get; init; }
    public required string Color { get; init; }
    public required decimal Price { get; init; }
    
    public required int Mileage { get; init; }
}

/// <summary> Спецификация прикрепления к <see cref="Car"/> другого фото </summary>
public record AttachCarPhotoSpec
{
    public required string Extension { get; init; }
    
    public required bool OnlyIfNonePhoto { get; init; }
}

/// <summary> Спецификация для назначения другого менеджера у <see cref="Car"/> </summary>
public record AssignCarManagerSpec
{
    public required Employer AssignmentManager { get; init; }
    
}