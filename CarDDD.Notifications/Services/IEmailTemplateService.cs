using CarDDD.Contracts.EmailContracts.EmailNotifications;
using CarDDD.Notifications.EmailTemplates;
using CarDDD.Notifications.Models.Email;

namespace CarDDD.Notifications.Services;

/// <summary>
/// Сервис подготовки Email почты.
/// Идейно вызывает для каждой реализации <see cref="IEmailNotification"/> соответствующий шаблон <see cref="IEmailTemplate{IEmailNotification}"/>,
/// и уже шаблон возвращает готовый <see cref="IEmailMessage"/>
/// </summary>
public interface IEmailTemplateService
{
    /// <summary>
    /// Создает Email письмо из Email оповещения
    /// </summary>
    public IEmailMessage? Create(IEmailNotification notification);
}