using Mediator;

namespace Norison.TransactionSync.Application.Features.Commands.Enable;

public class EnableCommand : ICommand
{
    public long ChatId { get; set; }
}