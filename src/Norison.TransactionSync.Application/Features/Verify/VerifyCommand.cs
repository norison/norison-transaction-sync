using MediatR;

namespace Norison.TransactionSync.Application.Features.Verify;

public class VerifyCommand : IRequest
{
    public long ChatId { get; set; }
}