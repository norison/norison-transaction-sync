using Norison.TransactionSync.Persistence.Attributes;

using Notion.Client;

namespace Norison.TransactionSync.Persistence.Storages.Models;

public class LogDbModel : IDbModel
{
    public string? Id { get; set; }
    public string? IconUrl { get; set; }

    [NotionProperty("Username", PropertyType.Title)]
    public string? Username { get; set; }

    [NotionProperty("Statement", PropertyType.RichText)]
    public string? Statement { get; set; }

    [NotionProperty("Transaction", PropertyType.RichText)]
    public string? Transaction { get; set; }
    
    [NotionProperty("Error", PropertyType.RichText)]
    public string? Error { get; set; }
}