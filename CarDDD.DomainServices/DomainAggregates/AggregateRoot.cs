using System.Collections.ObjectModel;
using CarDDD.DomainServices.BaseDomainEvents;
using CarDDD.DomainServices.EntityObjects;

namespace CarDDD.DomainServices.DomainAggregates;

public abstract class AggregateRoot<TId> : Entity<TId>
{
    private readonly List<IDomainEvent> _domainEvents = [];
    public ReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void AddDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);
    public void ClearAllDomainEvents() => _domainEvents.Clear();
}