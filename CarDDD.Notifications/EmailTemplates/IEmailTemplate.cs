using CarDDD.Contracts.EmailContracts.EmailNotifications;
using CarDDD.Notifications.Models.Email;

namespace CarDDD.Notifications.EmailTemplates;

/// <summary>
/// Тип шаблона для почтового письма 
/// </summary>
public interface IEmailTemplate<in T> where T : IEmailNotification
{
    /// <summary>
    /// Создание конкретного типа IEmailMessage через шаблон
    /// </summary>
    IEmailMessage Create(T notification);
}