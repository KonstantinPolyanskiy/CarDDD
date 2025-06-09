using System.Reflection;
using CarDDD.DomainServices.BaseDomainEvents;
using CarDDD.DomainServices.DomainAggregates;

namespace CarDDD.DomainTests.Shared;

public class DomainEventsCollector
{
    public static IReadOnlyList<IDomainEvent> GetEvents(AggregateRoot<Guid> aggr)
        => aggr.GetType()
               .BaseType!                     
               .GetField("_domainEvents", BindingFlags.Instance | BindingFlags.NonPublic)!
               .GetValue(aggr) as IReadOnlyList<IDomainEvent>
           ?? Array.Empty<IDomainEvent>();
}