using CarDDD.DomainServices.DomainAggregates.CartAggregate;
using CarDDD.DomainServices.DomainAggregates.CartAggregate.Events;
using CarDDD.DomainServices.EntityObjects;
using CarDDD.DomainServices.ValueObjects;
using CarDDD.DomainTests.Shared;

namespace CarDDD.DomainTests.Domain.Aggregates.CartTests;

public class CartCreateTests
{
    [Fact]
    public void CreateCart_Ok()
    {
        var customer = CustomerId.From(Guid.NewGuid());

        var result = Cart.Create(customer);
        var events = DomainEventsCollector.GetEvents(result); 
        
        result.Cars.Count.Should().Be(0);
        
        result.Ordered.Should().BeFalse();
        result.Purchased.Should().BeFalse();
        result.ReadyForPurchase.Should().BeFalse();
        
        result.CartOwnerId.Should().Be(customer);
        
        events.OfType<CartCreated>().Count().Should().Be(1);
    }
}