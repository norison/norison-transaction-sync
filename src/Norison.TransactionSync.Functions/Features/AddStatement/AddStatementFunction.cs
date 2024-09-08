using MediatR;

using Microsoft.Azure.Functions.Worker;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Norison.TransactionSync.Application.Features.Commands.AddStatement;

namespace Norison.TransactionSync.Functions.Features.AddStatement;

public class AddStatementFunction(ISender sender)
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
        
        var request = await req.ReadFromJsonAsync<AddStatementRequest>(cancellationToken: cancellationToken);
        
        if(request is null)
        {
            return new BadRequestResult();
        }

        var command = new AddStatementCommand
        {
            Account = request.Data.Account, StatementItem = request.Data.StatementItem
        };

        await sender.Send(command, cancellationToken);

        return new OkResult();
    }
}