using System.Text.Json.Serialization;

namespace Norison.TransactionSync.Functions.Models;

public class TransactionEvent
{
    [JsonPropertyName("chatId")] public long ChatId { get; set; }
    [JsonPropertyName("webHookData")] public string Data { get; set; } = string.Empty;
}