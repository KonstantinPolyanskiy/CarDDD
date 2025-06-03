using CarDDD.Notifications.Models.Email;

namespace CarDDD.Notifications.Services;

/// <summary>
/// Сервис для непосредственной отправки готового почтового сообщения
/// (Smtp, Unisender и т.д.)
/// </summary>
public interface IMailSender
{
    /// <summary>
    /// Отправить почту
    /// </summary>
    public Task<bool> SendAsync(IEmailMessage message);
}