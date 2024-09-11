using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

using Norison.BankNotionConnector.Application.Features.Commands.SetUser;
using Norison.BankNotionConnector.Persistence.Options;
using Norison.BankNotionConnector.Persistence.Storages;

using Telegram.Bot;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices((builder, services) =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        services.AddMediatR(options =>
        {
            options.Lifetime = ServiceLifetime.Singleton;
            options.RegisterServicesFromAssemblyContaining<SetUserCommand>();
        });

        services.AddMemoryCache();
        services.AddSingleton<IStorageFactory, StorageFactory>();
        
        services.Configure<StorageFactoryOptions>(options =>
            options.NotionToken = builder.Configuration["NotionAuthToken"]!);

        var telegramBotClient = new TelegramBotClient(builder.Configuration["TelegramBotToken"]!);
        telegramBotClient.SetWebhookAsync("https://profound-roughly-goshawk.ngrok-free.app/api/bot").Wait();
        services.AddSingleton<ITelegramBotClient>(telegramBotClient);
        
    })
    .Build();

host.Run();