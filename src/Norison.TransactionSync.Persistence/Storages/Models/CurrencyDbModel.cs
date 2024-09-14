using Norison.TransactionSync.Persistence.Attributes;

using Notion.Client;

namespace Norison.TransactionSync.Persistence.Storages.Models;

public class CurrencyDbModel : IDbModel
{
    public string? Id { get; set; }
    public string? IconUrl { get; set; }

    [NotionProperty("Name", PropertyType.Title)]
    public string? Name { get; set; }

    [NotionProperty("Leveler", PropertyType.Number)]
    public decimal? Leveler { get; set; }
}