using CarDDD.Core.EntityObjects;

namespace CarDDD.Core.DomainObjects.DomainCar;

public sealed class Photo : Entity<Guid>
{
    public bool Attached()
    {
        if (string.IsNullOrEmpty(Extension) || Data.Length == 0)
            return false;
        
        return true;
    }
    public string Extension { get; private init; } = string.Empty;
    public byte[] Data { get; init; } = [];
    
    public static Photo From(string ext, byte[] data) => new() 
        { Extension = ext, Data = data };
    
    public static Photo None => new();
}