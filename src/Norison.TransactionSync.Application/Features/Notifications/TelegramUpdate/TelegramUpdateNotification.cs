using Mediator;

using Telegram.Bot.Types;

namespace Norison.TransactionSync.Application.Features.Notifications.TelegramUpdate;

public class TelegramUpdateNotification : INotification
{
    public Update Update { get; set; } = new();
}