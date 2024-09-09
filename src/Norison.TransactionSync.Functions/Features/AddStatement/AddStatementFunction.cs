using Microsoft.Azure.Functions.Worker;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using Norison.TransactionSync.Functions.Options;

using Telegram.Bot;

namespace Norison.TransactionSync.Functions.Features.AddStatement;

public class AddStatementFunction(ITelegramBotClient telegramBotClient, IOptions<UsersOptions> usersOptions)
{
    [Function(nameof(AddStatementFunction))]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "statement")]
        HttpRequest req, CancellationToken cancellationToken)
    {
        if (req.Method == "GET")
        {
            return new OkResult();
        }

        var value = usersOptions.Value;
        
        var request = await req.ReadFromJsonAsync<AddStatementRequest>(cancellationToken: cancellationToken);
        
        if(request is null)
        {
            return new BadRequestResult();
        }

        const long chatId = 612177563;

        var accountId = request.Data.Account;
        var statement = request.Data.StatementItem;
        
        var date = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(statement.Time).ToLocalTime();
        
        var message = $"New statement for account {accountId}:\n" +
                      $"Date: {date}\n" +
                      $"Amount: {statement.Amount}\n" +
                      $"Description: {statement.Description}";
        
        await telegramBotClient.SendTextMessageAsync(chatId, message, cancellationToken: cancellationToken);

        return new OkResult();
    }
}