using MediatR;

namespace Norison.TransactionSync.Application.Features.SetMonoWebHook;

public class SetMonoWebHookCommand : IRequest
{
    public string Username { get; set; } = string.Empty;
}