namespace CarDDD.ApplicationServices.Models.StorageModels;

public class PhotoSnapshot
{
    public Guid Id { get; set; }
    
    public string Extension { get; set; }
    
    public byte[] Data { get; set; }
}