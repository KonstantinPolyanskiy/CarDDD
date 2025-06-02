using CarDDD.Core.AnswerObjects.Result;

namespace CarDDD.Infrastructure.Services.Mail;

public interface IMailSender
{
    public Task<Result<bool>> SendMailAsync(string to, string subject, string body);
}