using CarDDD.Core.AnswerObjects.Result;
using Microsoft.Extensions.Logging;

namespace CarDDD.Infrastructure.Services.Mail;

public class StubMailSender(ILogger<StubMailSender> log) : IMailSender
{
    public async Task<Result<bool>> SendMailAsync(string to, string subject, string body)
    {
        log.LogInformation("Отправка {mail} сообщения {body} с темой {subject}", to, body, subject);
        return await Task.FromResult(Result<bool>.Success(true));
    }
}