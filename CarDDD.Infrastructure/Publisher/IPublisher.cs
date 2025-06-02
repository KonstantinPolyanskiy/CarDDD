using CarDDD.Core.AnswerObjects.Result;
using CarDDD.Core.IntegrationEvents;

namespace CarDDD.Infrastructure.Publisher;

public interface IIntegrationPublisher
{
    public Task<Result<bool>> PublishAsync<T>(T message);
}