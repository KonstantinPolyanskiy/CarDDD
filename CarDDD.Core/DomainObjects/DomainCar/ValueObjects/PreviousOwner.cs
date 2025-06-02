namespace CarDDD.Core.DomainObjects.DomainCar.ValueObjects;

public sealed class Ownership
{
    public bool Exist()
    {
        if (string.IsNullOrEmpty(Name) && Mileage <= 0)
            return false;
        
        return true;
    }
    public string Name { get; private init; } = string.Empty;
    public int Mileage { get; private init; }

    public static Ownership PreviousOwnership(string name, int mileage) => new() 
        { Name = name, Mileage = mileage };
    
    public static Ownership None => new();
}