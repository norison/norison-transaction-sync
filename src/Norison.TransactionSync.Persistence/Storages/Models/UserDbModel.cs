using Norison.TransactionSync.Persistence.Attributes;

using Notion.Client;

namespace Norison.TransactionSync.Persistence.Storages.Models;

public class UserDbModel : IDbModel
{
    public string? Id { get; set; }

    [NotionProperty("Username", PropertyType.Title)]
    public string Username { get; set; } = string.Empty;

    [NotionProperty("ChatId", PropertyType.Number)]
    public long ChatId { get; set; }

    [NotionProperty("NotionToken", PropertyType.RichText)]
    public string NotionToken { get; set; } = string.Empty;

    [NotionProperty("MonoToken", PropertyType.RichText)]
    public string MonoToken { get; set; } = string.Empty;

    [NotionProperty("MonoAccountName", PropertyType.RichText)]
    public string MonoAccountName { get; set; } = string.Empty;

    [NotionProperty("Data", PropertyType.RichText)]
    public string Data { get; set; } = string.Empty;
}