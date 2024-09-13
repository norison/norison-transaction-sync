using MediatR;

namespace Norison.TransactionSync.Application.Features.Refresh;

public class RefreshCommand : IRequest
{
    public long ChatId { get; set; }
}