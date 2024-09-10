using System.Text.Json;

using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

using Norison.BankNotionConnector.Functions.Models;
using Norison.BankNotionConnector.Functions.Options;

using Telegram.Bot;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices((hostContext, services ) =>
    {
        services.AddLogging();
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        
        var telegramBotClient = new TelegramBotClient(hostContext.Configuration["TelegramBotToken"]!);
        //telegramBotClient.SetWebhookAsync("https://profound-roughly-goshawk.ngrok-free.app/api/bot").Wait();
        services.AddSingleton<ITelegramBotClient>(telegramBotClient);

        services.AddOptions<UsersOptions>().Configure(options =>
        {
            var userSettingsJson = hostContext.Configuration["UserSettings"]!;
            
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            
            options.UserSettings = JsonSerializer.Deserialize<UserSetting[]>(userSettingsJson, jsonOptions)!;
        });
    })
    .Build();

host.Run();