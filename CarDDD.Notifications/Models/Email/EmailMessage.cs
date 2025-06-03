namespace CarDDD.Notifications.Models.Email;

/// <summary>
/// Отправляемое Email письмо
/// </summary>
public interface IEmailMessage
{
    /// <summary>
    /// Адрес 
    /// </summary>
    public string To();
    
    /// <summary>
    /// Тема
    /// </summary>
    public string Subject();

    /// <summary>
    /// Тело письма (plain, html, с вложением и тд)
    /// </summary>
    public string Body();
}

public record HtmlEmailMessage(string To, string Subject, string HtmlBody) : IEmailMessage
{
    string IEmailMessage.To() => To;

    string IEmailMessage.Subject() => Subject;

    public string Body() => HtmlBody;
}
