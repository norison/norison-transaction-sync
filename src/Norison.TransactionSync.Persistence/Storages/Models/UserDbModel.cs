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

    [NotionProperty("MonoAccountId", PropertyType.RichText)]
    public string MonoAccountId { get; set; } = string.Empty;
    
    [NotionProperty("AccountsDatabaseId", PropertyType.RichText)]
    public string AccountsDatabaseId { get; set; } = string.Empty;
    
    [NotionProperty("BudgetsDatabaseId", PropertyType.RichText)]
    public string BudgetsDatabaseId { get; set; } = string.Empty;
    
    [NotionProperty("TransactionsDatabaseId", PropertyType.RichText)]
    public string TransactionsDatabaseId { get; set; } = string.Empty;
}