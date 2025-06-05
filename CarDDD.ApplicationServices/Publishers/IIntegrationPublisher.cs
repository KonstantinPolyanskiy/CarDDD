using CarDDD.ApplicationServices.Models.AnswerObjects.Result;

namespace CarDDD.ApplicationServices.Publishers;

public interface IIntegrationPublisher
{
    public Task<Result<bool>> PublishAsync<T>(T message);
}