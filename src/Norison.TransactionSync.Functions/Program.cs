using AlphaVantage.Net.Core.Client;
using AlphaVantage.Net.Stocks.Client;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

using Monobank.Client;

using Norison.TransactionSync.Application.Options;
using Norison.TransactionSync.Application.Services.Journal;
using Norison.TransactionSync.Application.Services.Messages;
using Norison.TransactionSync.Application.Services.Stocks;
using Norison.TransactionSync.Application.Services.Users;
using Norison.TransactionSync.Persistence.Options;
using Norison.TransactionSync.Persistence.Storages;

using Telegram.Bot;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices((builder, services) =>
    {
        services.AddMediator(options => options.ServiceLifetime = ServiceLifetime.Singleton);

        services.AddMemoryCache();
        services.AddSingleton(MonobankClientFactory.Create());
        services.AddSingleton<IStorageFactory, StorageFactory>();
        services.AddSingleton<IUsersService, UsersService>();
        services.AddSingleton<IMessagesService, MessagesService>();
        services.AddSingleton<IJournalService, JournalService>();

        services.Configure<StorageFactoryOptions>(options =>
            options.NotionToken = builder.Configuration["NotionAuthToken"]!);

        services.Configure<NotionOptions>(options =>
        {
            options.NotionUsersDatabaseId = builder.Configuration["NotionUsersDatabaseId"]!;
            options.NotionJournalsDatabaseId = builder.Configuration["NotionJournalsDatabaseId"]!;
        });

        services.Configure<WebHookOptions>(options =>
            options.WebHookBaseUrl = builder.Configuration["WebHookBaseUrl"]!);

        services.AddSingleton<ITelegramBotClient>(new TelegramBotClient(builder.Configuration["TelegramBotToken"]!));

        services.AddSingleton<IStocksService>(_ =>
            new StocksService(new AlphaVantageClient(builder.Configuration["AlphaVantageApiKey"]!).Stocks()));
    })
    .Build();

host.Run();