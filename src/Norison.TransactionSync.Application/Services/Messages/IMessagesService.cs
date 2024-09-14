namespace Norison.TransactionSync.Application.Services.Messages;

public interface IMessagesService
{
    Task SendMessageAsync(long chatId, string message, CancellationToken cancellationToken = default);
}