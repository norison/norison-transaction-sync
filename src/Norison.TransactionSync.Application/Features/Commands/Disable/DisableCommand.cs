using Mediator;

namespace Norison.TransactionSync.Application.Features.Commands.Disable;

public class DisableCommand : ICommand
{
    public long ChatId { get; set; }
}