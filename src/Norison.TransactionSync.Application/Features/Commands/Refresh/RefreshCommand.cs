using MediatR;

namespace Norison.TransactionSync.Application.Features.Commands.Refresh;

public class RefreshCommand : IRequest
{
    public long ChatId { get; set; }
}