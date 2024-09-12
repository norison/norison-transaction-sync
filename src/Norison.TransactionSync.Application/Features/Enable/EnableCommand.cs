using MediatR;

namespace Norison.TransactionSync.Application.Features.Enable;

public class EnableCommand : IRequest
{
    public long ChatId { get; set; }
}