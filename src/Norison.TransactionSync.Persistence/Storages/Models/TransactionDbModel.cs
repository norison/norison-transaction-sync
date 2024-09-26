using Norison.TransactionSync.Persistence.Attributes;
using Norison.TransactionSync.Persistence.Enums;
using Norison.TransactionSync.Persistence.Models;

using Notion.Client;

namespace Norison.TransactionSync.Persistence.Storages.Models;

public class TransactionDbModel : IDbModel
{
    public string? Id { get; set; }
    public string? IconUrl { get; set; }

    [NotionProperty("Name", PropertyType.Title)]
    public string? Name { get; set; }

    [NotionProperty("Type", PropertyType.Select)]
    public TransactionType? Type { get; set; }

    [NotionProperty("Date", PropertyType.Date)]
    public DateRange? Date { get; set; }

    [NotionProperty("Amount From", PropertyType.Number)]
    public decimal? AmountFrom { get; set; }

    [NotionProperty("Amount To", PropertyType.Number)]
    public decimal? AmountTo { get; set; }

    [NotionProperty("Notes", PropertyType.RichText)]
    public string? Notes { get; set; }

    [NotionProperty("Account From", PropertyType.Relation)]
    public string[] AccountFromIds { get; set; } = [];

    [NotionProperty("Account To", PropertyType.Relation)]
    public string[] AccountToIds { get; set; } = [];

    [NotionProperty("Category", PropertyType.Relation)]
    public string[] CategoryIds { get; set; } = [];

    [NotionProperty("Budget", PropertyType.Relation)]
    public string[] BudgetIds { get; set; } = [];
}