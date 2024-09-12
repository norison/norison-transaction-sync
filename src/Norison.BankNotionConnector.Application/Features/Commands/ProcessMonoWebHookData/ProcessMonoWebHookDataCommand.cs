using MediatR;

using Monobank.Client;

namespace Norison.BankNotionConnector.Application.Features.Commands.ProcessMonoWebHookData;

public class ProcessMonoWebHookDataCommand : IRequest
{
    public WebHookData WebHookData { get; set; } = new();
}