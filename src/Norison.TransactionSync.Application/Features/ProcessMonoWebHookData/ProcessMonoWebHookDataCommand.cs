using MediatR;

using Monobank.Client;

namespace Norison.TransactionSync.Application.Features.ProcessMonoWebHookData;

public class ProcessMonoWebHookDataCommand : IRequest
{
    public long ChatId { get; set; }
    public WebHookData WebHookData { get; set; } = new();
}