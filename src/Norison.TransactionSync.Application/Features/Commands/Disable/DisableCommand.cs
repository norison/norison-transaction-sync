using MediatR;

namespace Norison.TransactionSync.Application.Features.Commands.Disable;

public class DisableCommand : IRequest
{
    public long ChatId { get; set; }
}