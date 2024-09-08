using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Norison.TransactionSync.Functions.Features;

public class TestFunction(ILogger<TestFunction> logger)
{
    [Function(nameof(TestFunction))]
    public async Task<IActionResult> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "test")] HttpRequest req)
    {
        await Task.CompletedTask;
        logger.LogInformation("C# HTTP trigger function processed a request.");
        return new OkObjectResult("Welcome to Azure Functions! This HTTP triggered function executed successfully.");
    }
}