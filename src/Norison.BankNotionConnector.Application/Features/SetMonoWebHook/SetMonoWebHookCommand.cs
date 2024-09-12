using MediatR;

namespace Norison.BankNotionConnector.Application.Features.SetMonoWebHook;

public class SetMonoWebHookCommand : IRequest
{
    public string Username { get; set; } = string.Empty;
}