using MediatR;

using Microsoft.Extensions.Logging;

namespace Norison.TransactionSync.Application.Features.Commands.AddStatement;

public class AddStatementCommandHandler(ILogger<AddStatementCommandHandler> logger) : IRequestHandler<AddStatementCommand>
{
    public async Task Handle(AddStatementCommand request, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        
        logger.LogInformation("AddStatementCommandHandler");
    }
}