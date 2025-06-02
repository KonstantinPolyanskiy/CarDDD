using CarDDD.Core.DomainObjects.CommonValueObjects;

namespace CarDDD.Core.EntityObjects;

public sealed class Consumer : Entity<Guid>
{
    public static Consumer From(string firstName, string lastName) => new()
        { FirstName = firstName, LastName = lastName };

    public static Consumer From(ConsumerId consumerId, string firstName, string lastName) => new()
        { EntityId = consumerId.Value, FirstName = firstName, LastName = lastName };
    
    public string FirstName { get; private init; } = string.Empty;
    public string LastName { get; private init; } = string.Empty;
}