using System.Text.RegularExpressions;

using MediatR;

using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;

using Norison.BankNotionConnector.Application.Features.Commands.AddUser;
using Norison.BankNotionConnector.Application.Features.Commands.SetMonoWebHook;
using Norison.BankNotionConnector.Application.Features.Queries.GetUser;
using Norison.BankNotionConnector.Application.Features.Queries.Test;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Norison.BankNotionConnector.Functions.Features;

public class TelegramBotUpdateFunction(ITelegramBotClient telegramBotClient, ISender sender)
{
    [Function(nameof(TelegramBotUpdateFunction))]
    public async Task RunAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "bot")]
        HttpRequest req, CancellationToken cancellationToken)
    {
        try
        {
            var update = await req.ReadFromJsonAsync<Update>(JsonBotAPI.Options, cancellationToken);

            if (update?.Type != UpdateType.Message || update.Message?.Type != MessageType.Text)
            {
                return;
            }

            var text = update.Message.Text!;

            if (text == "/start")
            {
                await HandleStartAsync(update);
            }
            else if (text == "/test")
            {
                var query = new TestQuery();
                await sender.Send(query, cancellationToken);
            }
            else if (text == "/enablewebhook")
            {
                var command = new SetMonoWebHookCommand { Username = update.Message.Chat.Username! };
                await sender.Send(command, cancellationToken);
            }
            else if (text == "/getsettings")
            {
                var query = new GetUserQuery { Username = update.Message.Chat.Username! };

                var user = await sender.Send(query, cancellationToken);


                await telegramBotClient.SendTextMessageAsync(update.Message.Chat.Id,
                    $"Your settings:\n\nMonobank account name: {user.MonoAccountName}\nMonobank token: {user.MonoToken}\nNotion token: {user.NotionToken}",
                    cancellationToken: cancellationToken);
            }
            else if (text.StartsWith("/setsettings"))
            {
                var regex = new Regex(@"""([^""]*)""");
                var matches = regex.Matches(text);

                if (matches.Count != 3)
                {
                    await telegramBotClient.SendTextMessageAsync(update.Message.Chat.Id,
                        "Invalid format. Please provide your Notion integration token and Monobank token in the following format:\n\n/setsettings \"monobank-account-name\" \"monobank-token\" \"notion-token\"");
                    return;
                }

                var command = new AddUserCommand
                {
                    Username = update.Message.Chat.Username!,
                    ChatId = update.Message.Chat.Id,
                    MonoAccountName = matches[0].Groups[1].Value,
                    MonoToken = matches[1].Groups[1].Value,
                    NotionToken = matches[2].Groups[1].Value
                };

                await sender.Send(command);
            }
            else
            {
                await telegramBotClient.SendTextMessageAsync(update.Message.Chat.Id, "I don't understand you.");
            }
        }
        catch
        {
            // ignored
        }
    }

    private async Task HandleStartAsync(Update update)
    {
        var chatId = update.Message!.Chat.Id;
        var user = update.Message!.From!;

        var message = $"Hello {user.FirstName} {user.LastName}!\n\n" +
                      "I am a bot that helps you to connect your bank transactions to Notion.\n\n" +
                      "To get started, please provide your Notion integration token and Monobank token in the following format:\n\n" +
                      "/setsettings \"monobank-account-name\" \"monobank-token\" \"notion-token\"";

        await telegramBotClient.SendTextMessageAsync(chatId, message);
    }
}