namespace CarDDD.Core.SnapshotModels;

public class PhotoSnapshoteModel
{
    public Guid Id { get; set; }
    
    public string Extension { get; set; } = string.Empty;
    public required byte[] Data { get; set; } = [];
}