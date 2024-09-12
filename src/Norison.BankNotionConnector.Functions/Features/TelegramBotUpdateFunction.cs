using MediatR;

using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;

using Norison.BankNotionConnector.Application.Features.Disable;
using Norison.BankNotionConnector.Application.Features.Enable;
using Norison.BankNotionConnector.Application.Features.GetSettings;
using Norison.BankNotionConnector.Application.Features.SetSettings;
using Norison.BankNotionConnector.Application.Features.Verify;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Norison.BankNotionConnector.Functions.Features;

public class TelegramBotUpdateFunction(ITelegramBotClient client, ISender sender)
{
    private static readonly Dictionary<long, string> LastChatCommand = new();

    [Function(nameof(TelegramBotUpdateFunction))]
    public async Task RunAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "bot")]
        HttpRequest req, CancellationToken cancellationToken)
    {
        var update = await req.ReadFromJsonAsync<Update>(JsonBotAPI.Options, cancellationToken);

        if (update?.Type != UpdateType.Message || update.Message?.Type != MessageType.Text)
        {
            return;
        }

        try
        {
            var chatId = update.Message.Chat.Id;
            var username = update.Message.Chat.Username;
            var text = update.Message.Text!;

            if (text.StartsWith("/start"))
            {
                LastChatCommand[chatId] = "/start";
                await HandleStartCommandAsync(chatId, text, cancellationToken);
                return;
            }

            if (text.StartsWith("/setsettings"))
            {
                LastChatCommand[chatId] = "/setsettings";
                await HandleSetSettingsCommandAsync(chatId, text, cancellationToken);
                return;
            }

            if (text.StartsWith("/getsettings"))
            {
                var command = new GetSettingsCommand { Username = username!, ChatId = chatId };
                await sender.Send(command, cancellationToken);
                return;
            }

            if (text.StartsWith("/verify"))
            {
                var command = new VerifyCommand { ChatId = chatId };
                await sender.Send(command, cancellationToken);
                return;
            }

            if (text.StartsWith("/enable"))
            {
                var command = new EnableCommand { ChatId = chatId };
                await sender.Send(command, cancellationToken);
                return;
            }

            if (text.StartsWith("/disable"))
            {
                var command = new DisableCommand { ChatId = chatId };
                await sender.Send(command, cancellationToken);
                return;
            }

            if (LastChatCommand.TryGetValue(chatId, out var lastCommand) && lastCommand == "/setsettings")
            {
                var command = new SetSettingsCommand { Username = username!, ChatId = chatId, Text = text };
                await sender.Send(command, cancellationToken);
                return;
            }

            await client.SendTextMessageAsync(update.Message.Chat.Id, "I don't understand you.",
                cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            await client.SendTextMessageAsync(update.Message.Chat.Id, $"Error: {ex.Message}",
                cancellationToken: cancellationToken);
        }
    }

    private async Task HandleStartCommandAsync(long chatId, string text, CancellationToken cancellationToken)
    {
        const string message = "Hello! I'm a bot that can help you to sync your bank transactions with Notion.\n" +
                               "To start, please, enter the next command /setsettings";

        await client.SendTextMessageAsync(chatId, message, cancellationToken: cancellationToken);
    }

    private async Task HandleSetSettingsCommandAsync(long chatId, string text, CancellationToken cancellationToken)
    {
        const string message = "Enter your data in the following format:\n" +
                               "1. <notion-token>\n" +
                               "2. <mono-token>\n" +
                               "3. <mono-account-name>";

        await client.SendTextMessageAsync(chatId, message, cancellationToken: cancellationToken);
    }
}