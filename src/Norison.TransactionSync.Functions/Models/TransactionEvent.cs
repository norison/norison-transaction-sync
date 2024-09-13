using Monobank.Client;

namespace Norison.TransactionSync.Functions.Models;

public class TransactionEvent
{
    public long ChatId { get; set; }
    public WebHookData WebHookData { get; set; } = new();
}