using CarDDD.Core.DomainObjects.DomainCar;

namespace CarDDD.Core.SnapshotModels;

/// <summary>
/// Данные сущности Машина в базе данных
/// </summary>
public class CarSnapshotModel()
{
    public Guid Id { get; init; }
    
    public Guid? PhotoId { get; init; }
    
    public Guid ManagerId { get; init; }
    
    public string Brand { get; init; } = string.Empty;
    public string Color { get; init; } = string.Empty;
    public decimal Price { get; init; }
    
    public string? PreviousOwnerName { get; init; }
    public int? Mileage { get; init; }
    
    public Condition Condition { get; init; } 
    public bool IsAvailable { get; init; }
}