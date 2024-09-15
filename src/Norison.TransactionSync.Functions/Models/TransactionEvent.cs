using System.Text.Json.Serialization;

using Monobank.Client;

namespace Norison.TransactionSync.Functions.Models;

public class TransactionEvent
{
    [JsonPropertyName("chatId")] public long ChatId { get; set; }
    [JsonPropertyName("webHookData")] public WebHookData Data { get; set; } = new();
}