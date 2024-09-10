using Microsoft.Azure.Functions.Worker;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Norison.BankNotionConnector.Functions.Features;

public class TelegramBotUpdate(ITelegramBotClient telegramBotClient)
{
    [Function(nameof(TelegramBotUpdate))]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "bot")] HttpRequest req)
    {
        var update = await req.ReadFromJsonAsync<Update>(JsonBotAPI.Options);
        
        if(update is null || update.Type != UpdateType.Message || update.Message is null)
        {
            return new OkResult();
        }

        if (update.Message.Text == "/start")
        {
            await telegramBotClient.SendTextMessageAsync(update.Message.Chat.Id, $"Hello! I'm a bot. How can I help you?");
        }
        
        await telegramBotClient.SendTextMessageAsync(update.Message.Chat.Id, $"Your message: {update.Message.Text}");

        return new OkResult();
    }
}