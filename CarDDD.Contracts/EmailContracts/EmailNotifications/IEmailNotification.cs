namespace CarDDD.Contracts.EmailContracts.EmailNotifications;

/// <summary>
/// Оповещение по электронной почте
/// </summary>
public interface IEmailNotification
{
    /// <summary>
    /// Адрес электронной почты
    /// </summary>
    public string To();
}