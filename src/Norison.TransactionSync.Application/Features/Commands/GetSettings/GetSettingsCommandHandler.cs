using MediatR;

using Norison.TransactionSync.Application.Services.UserInfos;

using Telegram.Bot;

namespace Norison.TransactionSync.Application.Features.Commands.GetSettings;

public class GetSettingsCommandHandler(
    IUserInfosService userInfosService,
    ITelegramBotClient client) : IRequestHandler<GetSettingsCommand>
{
    public async Task Handle(GetSettingsCommand request, CancellationToken cancellationToken)
    {
        var user = await userInfosService.GetUserAsync(request.ChatId, cancellationToken);

        if (user is null)
        {
            await client.SendTextMessageAsync(request.ChatId, "User not found", cancellationToken: cancellationToken);
            return;
        }

        var message = $"Notion token: {user.NotionToken}\n" +
                      $"Mono token: {user.MonoToken}\n" +
                      $"Mono account name: {user.MonoAccountName}";

        await client.SendTextMessageAsync(request.ChatId, message, cancellationToken: cancellationToken);
    }
}