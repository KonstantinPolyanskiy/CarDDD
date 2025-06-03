using CarDDD.Contracts.EmailContracts.EmailNotifications;
using CarDDD.Notifications.EmailTemplates;
using CarDDD.Notifications.Models.Email;

namespace CarDDD.Notifications.Services.Implementations;

/// <summary>
/// <see cref="IEmailTemplateService"/> 
/// </summary>
public class SimpleHtmlEmailTemplateService(IServiceProvider serviceProvider, ILogger<SimpleHtmlEmailTemplateService> log) : IEmailTemplateService
{
    public IEmailMessage? Create(IEmailNotification notification)
    {
        var templateType = typeof(IEmailTemplate<>).MakeGenericType(notification.GetType());
        
        var template = serviceProvider.GetService(templateType);
        if (template == null)
        {
            log.LogWarning("Не найден шаблон для письма с типом {templateType} для уведомления {notification}", templateType, notification.GetType());
            return null;
        }

        return (IEmailMessage)templateType
            .GetMethod(nameof(IEmailTemplate<IEmailNotification>.Create))!
            .Invoke(template, new object[] { notification })!;
    }
}