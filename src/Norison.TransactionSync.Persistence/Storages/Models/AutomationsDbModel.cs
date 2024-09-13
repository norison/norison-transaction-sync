using Norison.TransactionSync.Persistence.Attributes;

using Notion.Client;

namespace Norison.TransactionSync.Persistence.Storages.Models;

public class AutomationsDbModel : IDbModel
{
    public string? Id { get; set; }

    [NotionProperty("Name", PropertyType.Title)]
    public string? Name { get; set; }
}