using System.Text.RegularExpressions;

using MediatR;

using Norison.BankNotionConnector.Persistence.Storages;
using Norison.BankNotionConnector.Persistence.Storages.Models;

using Notion.Client;

using Telegram.Bot;

namespace Norison.BankNotionConnector.Application.Features.SetSettings;

public class SetSettingsCommandHandler(IStorageFactory storageFactory, ITelegramBotClient client)
    : IRequestHandler<SetSettingsCommand>
{
    public async Task Handle(SetSettingsCommand request, CancellationToken cancellationToken)
    {
        const string pattern = @"^1\.\s.+\r?\n2\.\s.+\r?\n3\.\s.+$";
        var isValid = Regex.IsMatch(request.Text, pattern, RegexOptions.Multiline);

        if (!isValid)
        {
            await client.SendTextMessageAsync(request.ChatId, "Invalid settings format.",
                cancellationToken: cancellationToken);
            return;
        }

        var lines = request.Text.Split("\n");
        var notionToken = lines[0].Split(".")[1].Trim();
        var monoToken = lines[1].Split(".")[1].Trim();
        var monoAccountName = lines[2].Split(".")[1].Trim();

        var userStorage = storageFactory.GetUsersStorage();

        var parameters = new DatabasesQueryParameters { Filter = new TitleFilter("Username", request.Username) };
        var user = await userStorage.GetFirstAsync(parameters, cancellationToken);

        if (user is not null)
        {
            user.NotionToken = notionToken;
            user.MonoToken = monoToken;
            user.MonoAccountName = monoAccountName;
            await userStorage.UpdateAsync(user, cancellationToken);
        }
        else
        {
            var userDbModel = new UserDbModel
            {
                Username = request.Username,
                ChatId = request.ChatId,
                NotionToken = notionToken,
                MonoToken = monoToken,
                MonoAccountName = monoAccountName
            };

            await userStorage.AddAsync(userDbModel, cancellationToken);
        }


        await client.SendTextMessageAsync(request.ChatId, "Settings saved successfully.",
            cancellationToken: cancellationToken);
    }
}