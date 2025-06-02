namespace CarDDD.Core.DomainObjects.DomainCar.ValueObjects;

public sealed class Photo
{
    public Guid? Id { get; private init; }
    public bool Attached()
    {
        if (string.IsNullOrEmpty(Extension) || Data.Length == 0)
            return false;
        
        return true;
    }
    public string Extension { get; private init; } = string.Empty;
    public byte[] Data { get; private init; } = [];
    
    public static Photo From(string ext, byte[] data) => new() 
        { Id = Guid.NewGuid(), Extension = ext, Data = data };
    
    public static Photo None => new() {Id = null};
}