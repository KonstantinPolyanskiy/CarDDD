using CarDDD.Notifications.Models.Email;

namespace CarDDD.Notifications.Services.Implementations;

public class StubMailSenderService(ILogger<StubMailSenderService> log) : IMailSender
{
    public async Task<bool> SendAsync(IEmailMessage message)
    {
        log.LogInformation("Отправлено почтовое сообщение на {address}, с телом {body} и темой {subject}", message.To, message.Subject, message.Body); 
        
        return await Task.FromResult(true);
    }
}