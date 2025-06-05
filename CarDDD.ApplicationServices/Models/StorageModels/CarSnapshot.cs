namespace CarDDD.ApplicationServices.Models.StorageModels;

/// <summary>
/// Таблица Машина в БД
/// </summary>
public class CarSnapshot
{
    public Guid Id { get; set; }
    
    public Guid PhotoId { get; set; }
    public string PhotoExtension { get; set; } = string.Empty;
    
    public Guid ManagerId { get; set; }
    
    public string Brand { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Mileage { get; set; }
    
    // Наверное хранить состояние машины не совсем корректно 
    // Оно меняется согласно БЛ, и рассчитывается через доменную модель
    /*public Condition Condition { get; set; }*/ 
    
    public bool IsAvailable { get; set; }
}