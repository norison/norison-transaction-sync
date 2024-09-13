using System.Text.RegularExpressions;

using MediatR;

using Norison.TransactionSync.Application.Services.UserInfos;

using Telegram.Bot;

namespace Norison.TransactionSync.Application.Features.Commands.SetSettings;

public class SetSettingsCommandHandler(ITelegramBotClient telegramBotClient, IUserInfosService userInfosService)
    : IRequestHandler<SetSettingsCommand>
{
    public async Task Handle(SetSettingsCommand request, CancellationToken cancellationToken)
    {
        const string pattern = @"^1\.\s.+\r?\n2\.\s.+\r?\n3\.\s.+$";
        var isValid = Regex.IsMatch(request.Text, pattern, RegexOptions.Multiline);

        if (!isValid)
        {
            await telegramBotClient.SendTextMessageAsync(request.ChatId, "Invalid settings format.",
                cancellationToken: cancellationToken);
            return;
        }

        var lines = request.Text.Split("\n");
        var notionToken = lines[0].Split(".")[1].Trim();
        var monoToken = lines[1].Split(".")[1].Trim();
        var monoAccountName = lines[2].Split(".")[1].Trim();

        await userInfosService.PopulateUserAsync(
            request.ChatId,
            request.Username,
            monoToken,
            notionToken,
            monoAccountName,
            cancellationToken);

        await telegramBotClient.SendTextMessageAsync(request.ChatId, "Settings saved successfully.",
            cancellationToken: cancellationToken);
    }
}