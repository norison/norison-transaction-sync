using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

using Norison.TransactionSync.Application.Features.Commands.AddStatement;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddLogging();
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblyContaining<AddStatementCommand>();
            cfg.Lifetime = ServiceLifetime.Singleton;
        });
    })
    .Build();

host.Run();