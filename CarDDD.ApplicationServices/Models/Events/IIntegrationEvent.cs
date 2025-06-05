namespace CarDDD.ApplicationServices.Models.Events;

public interface IIntegrationEvent
{
    DateTime OccurredOn { get; }
}