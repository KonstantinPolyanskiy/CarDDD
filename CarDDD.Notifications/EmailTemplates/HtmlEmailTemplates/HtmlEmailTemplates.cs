using CarDDD.Contracts.EmailContracts.EmailNotifications;
using CarDDD.Notifications.Models.Email;

namespace CarDDD.Notifications.EmailTemplates.HtmlEmailTemplates;

public sealed class CarCreatedWithoutPhotoHtmlTemplate :
    IEmailTemplate<ManagerCreatedCarWithoutPhotoNotification>
{
    public IEmailMessage Create(ManagerCreatedCarWithoutPhotoNotification m) =>
        new HtmlEmailMessage(m.To(), "Машина без фотографии", HtmlBody:
             $"""
             <p>{m.ManagerFullName}, сегодня вы добавили машину <b>{m.CarId}</b> без фотографии.</p>
             <p>Пожалуйста, прикрепите фото, чтобы объявление попало в поиск.</p>
             """);
}

public sealed class ConsumerOrderedCartInfoHtmlTemplate :
    IEmailTemplate<ConsumerOrderedCartInfoEmailNotification>
{
    public IEmailMessage Create(ConsumerOrderedCartInfoEmailNotification m) =>
        new HtmlEmailMessage(m.To(), "Ваш заказ оформлен", HtmlBody:
             $"""
             <p>{m.CustomerFullName}, спасибо за заказ!</p>
             <p>Количество товаров: {m.TotalCount}<br/>
                Итоговая сумма: {m.TotalPrice:C}</p>
             """);
}

public sealed class EmployerOrderedCartInfoTemplate :
    IEmailTemplate<EmployerOrderedCartInfoEmailNotification>
{
    public IEmailMessage Create(EmployerOrderedCartInfoEmailNotification n)
    {
        var carsByManager = string.Join("<br/>",
            n.OrderedCars.Select(mc =>
                $"<b>{mc.ManagerId}</b>: {string.Join(", ", mc.CarIds)}"));

        var body = $"""
                    <p>Здравствуйте!</p>
                    <p>Клиент <b>{n.PurchaserFullName}</b> оформил заказ 
                       на <b>{n.TotalCount}</b> машин общей стоимостью <b>{n.TotalPrice}</b>.</p>
                    <p><u>Распределение машин по менеджерам:</u><br/>
                       {carsByManager}</p>
                    """;

        return new HtmlEmailMessage(
            To:       n.To(),
            Subject:  $"Заказ на {n.TotalPrice}",
            HtmlBody: body);
    }
}

