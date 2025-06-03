using CarDDD.Core.AnswerObjects.Result;

namespace CarDDD.Infrastructure.Publisher;

public interface IIntegrationPublisher
{
    public Task<Result<bool>> PublishAsync<T>(T message);
}