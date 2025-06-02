using System.Collections.ObjectModel;
using CarDDD.Core.DomainEvents;
using CarDDD.Core.EntityObjects;

namespace CarDDD.Core.DomainObjects;

public abstract class AggregateRoot<TId> : Entity<TId>
{
    private readonly List<IDomainEvent> _domainEvents = [];
    public ReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void AddDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);
    public void ClearAllDomainEvents() => _domainEvents.Clear();
}