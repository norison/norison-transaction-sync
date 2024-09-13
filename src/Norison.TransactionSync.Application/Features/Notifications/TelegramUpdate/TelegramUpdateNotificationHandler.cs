using MediatR;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Norison.TransactionSync.Application.Features.Notifications.TelegramUpdate;

public class TelegramUpdateNotificationHandler(ITelegramBotClient client, ISender sender)
    : INotificationHandler<TelegramUpdateNotification>
{
    public async Task Handle(TelegramUpdateNotification notification, CancellationToken cancellationToken)
    {
        if (!ShouldHandleUpdate(notification.Update))
        {
            return;
        }

        var message = notification.Update.Message!;
        var chatId = message.Chat.Id;
        var text = message.Text!;

        try
        {
            await HandleUpdateAsync(chatId, text, cancellationToken);
        }
        catch (Exception exception)
        {
            await client.SendTextMessageAsync(chatId,
                "An error occurred while processing your request. Error: " + exception.Message,
                cancellationToken: cancellationToken);
        }
    }

    private async Task HandleUpdateAsync(long chatId, string text, CancellationToken cancellationToken)
    {
        switch (text)
        {
            case "/start":
                await HandleStartCommandAsync(chatId, text, cancellationToken);
                break;
        }
    }

    private async Task HandleStartCommandAsync(long chatId, string text, CancellationToken cancellationToken)
    {
    }

    private static bool ShouldHandleUpdate(Update update)
    {
        return update is { Type: UpdateType.Message, Message.Type: MessageType.Text };
    }
}