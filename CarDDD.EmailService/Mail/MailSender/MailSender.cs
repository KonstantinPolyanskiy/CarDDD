namespace CarDDD.EmailService.Mail.MailSender;

public interface IMailSender
{
    public Task<bool> SendMailAsync(string to, string subject, string body);
}

public class StubMailSender(ILogger<StubMailSender> log) : IMailSender
{
    public async Task<bool> SendMailAsync(string to, string subject, string body)
    {
        log.LogInformation("Отправка {mail} сообщения {body} с темой {subject}", to, body, subject);
        return await Task.FromResult(true);
    }
}