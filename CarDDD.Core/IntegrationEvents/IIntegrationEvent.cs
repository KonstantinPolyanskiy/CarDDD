namespace CarDDD.Core.IntegrationEvents;

public enum IntegrationEvent
{
    Email,
}


public interface IIntegrationEvent<TPayload>
{
    TPayload Payload { get; }
}

