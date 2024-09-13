using Azure.Messaging.ServiceBus;

using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

using Monobank.Client;

using Norison.TransactionSync.Application.Features.Commands.SetSettings;
using Norison.TransactionSync.Application.Options;
using Norison.TransactionSync.Application.Services.UserInfos;
using Norison.TransactionSync.Persistence.Options;
using Norison.TransactionSync.Persistence.Storages;

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
            options.RegisterServicesFromAssemblyContaining<SetSettingsCommand>();
        });

        services.AddMemoryCache();
        services.AddSingleton<IStorageFactory, StorageFactory>();
        services.AddSingleton(MonobankClientFactory.Create());
        services.AddSingleton<IUserInfosService, UserInfosService>();

        services.Configure<StorageFactoryOptions>(options =>
            options.NotionToken = builder.Configuration["NotionAuthToken"]!);

        services.Configure<NotionOptions>(options =>
            options.NotionUsersDatabaseId = builder.Configuration["NotionUsersDatabaseId"]!);

        services.Configure<WebHookOptions>(options =>
            options.WebHookBaseUrl = builder.Configuration["WebHookBaseUrl"]!);

        services.AddSingleton<ITelegramBotClient>(new TelegramBotClient(builder.Configuration["TelegramBotToken"]!));

        services.AddSingleton(new ServiceBusClient(builder.Configuration["ServiceBusConnectionString"]));
    })
    .Build();

host.Run();