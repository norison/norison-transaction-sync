using Norison.TransactionSync.Persistence.Attributes;

using Notion.Client;

namespace Norison.TransactionSync.Persistence.Storages.Models;

public class AccountDbModel : IDbModel
{
    public string? Id { get; set; }
    
    [NotionProperty("Name", PropertyType.Title)]
    public string? Name { get; set; }
}