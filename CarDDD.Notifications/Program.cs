using CarDDD.Contracts.EmailContracts.EmailNotifications;
using CarDDD.Notifications.Consumers;
using CarDDD.Notifications.EmailTemplates;
using CarDDD.Notifications.Services;
using CarDDD.Notifications.Services.Implementations;
using CarDDD.Settings.RabbitSettings;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

var builder = Host.CreateApplicationBuilder(args);

// Настройки подключения к RabbitMQ
builder.Services.Configure<RabbitConnectionSettings>(
    builder.Configuration.GetSection(nameof(RabbitConnectionSettings)));

// Настройки политики повторов RabbitMQ
builder.Services.Configure<RabbitRetryPolicy>(
    builder.Configuration.GetSection(nameof(RabbitRetryPolicy)));

// Добавляем RabbitMQ Connection
builder.Services.AddSingleton<IConnection>(st =>
{
    var opt = st.GetService<IOptions<RabbitConnectionSettings>>()!.Value;

    var factory = new ConnectionFactory
    {
        HostName = opt.HostName,
        UserName = opt.UserName,
        Password = opt.Password,
    };

    return factory.CreateConnectionAsync(opt.RabbitClientName).Result;
});

// Определяем для каждого IEmailTemplate свою реализацию
builder.Services.Scan(s => s
    .FromAssemblyOf<IEmailTemplate<IEmailNotification>>()
    .AddClasses(c => c.AssignableTo(typeof(IEmailTemplate<>)))
    .AsImplementedInterfaces()
    .WithScopedLifetime());

// Сервисы-зависимости
builder.Services.AddScoped<IEmailTemplateService, SimpleHtmlEmailTemplateService>();
builder.Services.AddScoped<IMailSender, StubMailSenderService>();

// Hosted-сервисы
builder.Services.AddHostedService<RabbitEmailConsumer>();

var host = builder.Build();
host.Run();