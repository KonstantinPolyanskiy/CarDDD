namespace CarDDD.ApplicationServices.Models.StorageModels;

/// <summary>
/// Таблица Корзина в БД
/// </summary>
public class CartSnapshot
{
    public Guid Id { get; set; }

    public IList<Guid> Cars { get; set; } = [];

    public Guid CartOwnerId { get; set; } 
    
    public bool Ordered { get; set; }
    
    public bool ReadyForPurchase { get; set; }
    
    public bool Purchased { get; set; }
}