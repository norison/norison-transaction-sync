using MediatR;

namespace Norison.TransactionSync.Application.Features.Disable;

public class DisableCommand : IRequest
{
    public long ChatId { get; set; }
}