using Telegram.Bot;

namespace Norison.TransactionSync.Application.Services.Messages;

public class MessagesService(ITelegramBotClient client) : IMessagesService
{
    public Task SendMessageAsync(long chatId, string message, CancellationToken cancellationToken = default)
    {
        return client.SendTextMessageAsync(chatId, message, cancellationToken: cancellationToken);
    }
}