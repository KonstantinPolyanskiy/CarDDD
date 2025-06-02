using CarDDD.EmailService.Mail.MailSender;

namespace CarDDD.EmailService.Mail.MailService;

public class MailService(IMailSender sender)
{
    public Task<bool> SendMessageAddedWithoutPhoto()
    {
        
    }
}