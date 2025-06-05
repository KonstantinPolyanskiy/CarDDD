namespace CarDDD.ApplicationServices.Models.StorageModels;

public class PhotoData
{
    public Guid Id { get; set; }
    
    public string Extension { get; set; }
    
    public byte[] Data { get; set; }
}