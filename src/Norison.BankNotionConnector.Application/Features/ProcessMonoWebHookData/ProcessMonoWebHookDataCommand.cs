using MediatR;

using Monobank.Client;

namespace Norison.BankNotionConnector.Application.Features.ProcessMonoWebHookData;

public class ProcessMonoWebHookDataCommand : IRequest
{
    public long ChatId { get; set; }
    public WebHookData WebHookData { get; set; } = new();
}