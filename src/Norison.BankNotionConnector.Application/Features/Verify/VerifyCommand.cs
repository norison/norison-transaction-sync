using MediatR;

namespace Norison.BankNotionConnector.Application.Features.Verify;

public class VerifyCommand : IRequest
{
    public long ChatId { get; set; }
}