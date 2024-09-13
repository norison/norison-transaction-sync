using MediatR;

namespace Norison.TransactionSync.Application.Features.Commands.Enable;

public class EnableCommand : IRequest
{
    public long ChatId { get; set; }
}