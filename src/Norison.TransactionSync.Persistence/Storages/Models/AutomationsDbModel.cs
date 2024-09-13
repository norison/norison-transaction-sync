using Norison.TransactionSync.Persistence.Attributes;

using Notion.Client;

namespace Norison.TransactionSync.Persistence.Storages.Models;

public class AutomationsDbModel : IDbModel
{
    public string? Id { get; set; }

    [NotionProperty("Name", PropertyType.Title)]
    public string? Name { get; set; }

    [NotionProperty("Description Is", PropertyType.RichText)]
    public string? DescriptionIs { get; set; }

    [NotionProperty("Description Contains", PropertyType.RichText)]
    public string? DescriptionContains { get; set; }

    [NotionProperty("Description Override", PropertyType.RichText)]
    public string? DescriptionOverride { get; set; }

    [NotionProperty("Transaction Type", PropertyType.Select)]
    public TransactionType? TransactionType { get; set; }

    [NotionProperty("Category", PropertyType.Relation)]
    public string[] CategoryIds { get; set; } = [];

    [NotionProperty("Account To", PropertyType.Relation)]
    public string[] AccountToIds { get; set; } = [];
}