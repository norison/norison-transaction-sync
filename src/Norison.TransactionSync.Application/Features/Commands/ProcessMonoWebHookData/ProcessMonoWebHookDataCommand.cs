using Mediator;

using Monobank.Client;

namespace Norison.TransactionSync.Application.Features.Commands.ProcessMonoWebHookData;

public class ProcessMonoWebHookDataCommand : ICommand
{
    public long ChatId { get; set; }
    public WebHookData WebHookData { get; set; } = new();
}