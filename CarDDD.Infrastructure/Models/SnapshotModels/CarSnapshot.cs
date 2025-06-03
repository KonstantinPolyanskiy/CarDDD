using CarDDD.Core.DomainObjects.DomainCar.ValueObjects;

namespace CarDDD.Infrastructure.Models.SnapshotModels;

/// <summary>
/// Таблица Машина в БД
/// </summary>
public class CarSnapshot
{
    public Guid Id { get; set; }
    
    public Guid? PhotoId { get; set; }
    
    public Guid ManagerId { get; set; }
    
    public string Brand { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public decimal Price { get; set; }
    
    public string PreviousOwnerName { get; set; }
    public int Mileage { get; set; }
    
    public Condition Condition { get; set; } 
    public bool IsAvailable { get; set; }
}