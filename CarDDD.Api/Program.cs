using System.Text.Json.Serialization;
using CarDDD.Api.Middlewares;
using CarDDD.ApplicationServices.EventHandlers.DomainEventHandlers.CarHandlers;
using CarDDD.ApplicationServices.EventHandlers.DomainEventHandlers.CartHandler;
using CarDDD.ApplicationServices.Extensions;
using CarDDD.Infrastructure.Contexts;
using CarDDD.Infrastructure.Extensions;
using CarDDD.Settings.KeycloakSettings;
using CarDDD.Settings.MinioSettings;
using CarDDD.Settings.RabbitSettings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Minio;
using RabbitMQ.Client;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

#region Postgres Connection

var pgConnection = builder.Configuration.GetConnectionString("DefaultConnection");

#endregion

#region Logger

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.WithProperty("Application", builder.Environment.ApplicationName)
    .CreateLogger();

builder.Host.UseSerilog((ctx, services, cfg) =>
{
    cfg.ReadFrom.Configuration(ctx.Configuration)     
        .ReadFrom.Services(services)                   
        .Enrich.FromLogContext()
        .WriteTo.Console();
});

#endregion

#region SettingsSection

builder.Services.Configure<MinioSettings>(
    builder.Configuration.GetSection("MinioSettings")
);

builder.Services.Configure<KeycloakSettings>(
    builder.Configuration.GetSection("KeycloakSettings")
);

builder.Services.Configure<RabbitConnectionSettings>(
    builder.Configuration.GetSection("RabbitSettings")
);

#endregion

#region Minio

builder.Services.AddSingleton<IMinioClient>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<MinioSettings>>();
    var minioBuilder = new MinioClient()
        .WithEndpoint(settings.Value.Endpoint, settings.Value.Port)
        .WithCredentials(settings.Value.AccessKey, settings.Value.SecretKey);

    if (settings.Value.UseSSL)
        minioBuilder = minioBuilder.WithSSL();

    return minioBuilder.Build();
});

#endregion

#region Rabbit

builder.Services.AddSingleton<IConnection>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<RabbitConnectionSettings>>();
    var rabbitFactory = new ConnectionFactory
    {
        HostName = settings.Value.HostName,
        UserName = settings.Value.UserName,
        Password = settings.Value.Password,
    };

    return rabbitFactory.CreateConnectionAsync().Result;
});

#endregion

#region Infrastructure 

builder.Services.AddApplicationDbContext(pgConnection!);

builder.Services.AddPublishers();

builder.Services.AddStorages();

#endregion

#region ApplicationServices

builder.Services.AddDomainHandlers(
    typeof(CarCreatedHandler).Assembly,
    typeof(CarPhotoAttachedHandler).Assembly,
    typeof(CarSoldHandler).Assembly,
    typeof(CarUpdatedBasisAttributesHandler).Assembly,
    typeof(CarManagerChangedHandler).Assembly,
    typeof(AddedCarInCartHandler).Assembly,
    typeof(RemovedCarFromCartHandler).Assembly,
    typeof(CartCreatedHandler).Assembly,
    typeof(CartOrderedHandler).Assembly,
    typeof(CartPurchasedHandler).Assembly
);

builder.Services.AddApplicationServices();

#endregion

#region Controllers

builder.Services.AddControllers()
    .AddJsonOptions(opt => opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication();

builder.Services.AddAuthorization();

#endregion

#region BuildApplication

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await db.Database.MigrateAsync();
}

app.UseMiddleware<CorrelationIdMiddleware>();

app.UseSerilogRequestLogging(opts =>
{
    opts.GetLevel = (ctx, _, ex) =>
        ex != null || ctx.Response.StatusCode >= 500
            ? LogEventLevel.Error
            : LogEventLevel.Information;
});

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();

#endregion



