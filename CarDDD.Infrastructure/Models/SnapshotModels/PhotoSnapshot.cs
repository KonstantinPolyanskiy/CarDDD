namespace CarDDD.Infrastructure.Models.SnapshotModels;

/// <summary>
/// Таблица Фото в БД
/// </summary>
public class PhotoSnapshot
{
    public Guid Id { get; set; }
    
    public string Extension { get; set; } = string.Empty;
    public required byte[] Data { get; set; } = [];
}