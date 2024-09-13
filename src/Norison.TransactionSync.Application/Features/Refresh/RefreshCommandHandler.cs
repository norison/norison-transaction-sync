using MediatR;

using Norison.TransactionSync.Application.Services.UserInfos;

using Telegram.Bot;

namespace Norison.TransactionSync.Application.Features.Refresh;

public class RefreshCommandHandler(IUserInfosService userInfosService, ITelegramBotClient telegramBotClient)
    : IRequestHandler<RefreshCommand>
{
    public async Task Handle(RefreshCommand request, CancellationToken cancellationToken)
    {
        await userInfosService.RefreshUserAsync(request.ChatId, cancellationToken);

        await telegramBotClient.SendTextMessageAsync(request.ChatId, "Refresh command executed successfully.",
            cancellationToken: cancellationToken);
    }
}