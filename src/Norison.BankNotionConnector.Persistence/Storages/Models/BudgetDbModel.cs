using Norison.BankNotionConnector.Persistence.Attributes;

using Notion.Client;

namespace Norison.BankNotionConnector.Persistence.Storages.Models;

public class BudgetDbModel : IDbModel
{
    public string? Id { get; set; }
    
    [NotionProperty("Name", PropertyType.Title)]
    public string? Name { get; set; }
    
    [NotionProperty("Budget", PropertyType.Number)]
    public decimal? Budget { get; set; }
    
    [NotionProperty("Transactions", PropertyType.Relation)]
    public string[] TransactionIds { get; set; } = [];
}