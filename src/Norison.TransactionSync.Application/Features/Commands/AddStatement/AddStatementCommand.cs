using MediatR;

using Norison.MonoSdk.Models;

namespace Norison.TransactionSync.Application.Features.Commands.AddStatement;

public class AddStatementCommand : IRequest
{
    public string Account { get; set; } = string.Empty;
    public Statement StatementItem { get; set; } = new();
}