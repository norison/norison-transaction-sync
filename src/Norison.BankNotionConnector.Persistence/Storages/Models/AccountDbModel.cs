using Norison.BankNotionConnector.Persistence.Attributes;

using Notion.Client;

namespace Norison.BankNotionConnector.Persistence.Storages.Models;

public class AccountDbModel : IDbModel
{
    public string? Id { get; set; }
    
    [NotionProperty("Name", PropertyType.Title)]
    public string? Name { get; set; }
}