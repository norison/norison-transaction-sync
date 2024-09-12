using MediatR;

namespace Norison.BankNotionConnector.Application.Features.Enable;

public class EnableCommand : IRequest
{
    public long ChatId { get; set; }
}