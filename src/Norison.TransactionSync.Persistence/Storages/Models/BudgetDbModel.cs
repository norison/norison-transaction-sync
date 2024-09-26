using Norison.TransactionSync.Persistence.Attributes;
using Norison.TransactionSync.Persistence.Models;

using Notion.Client;

namespace Norison.TransactionSync.Persistence.Storages.Models;

public class BudgetDbModel : IDbModel
{
    public string? Id { get; set; }
    public string? IconUrl { get; set; }

    [NotionProperty("Name", PropertyType.Title)]
    public string? Name { get; set; }

    [NotionProperty("Date Range", PropertyType.Date)]
    public DateRange? DateRange { get; set; }

    [NotionProperty("Budget", PropertyType.Number)]
    public decimal? Budget { get; set; }

    [NotionProperty("Transactions", PropertyType.Relation)]
    public string[] TransactionIds { get; set; } = [];
}