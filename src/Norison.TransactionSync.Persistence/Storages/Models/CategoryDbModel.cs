using Norison.TransactionSync.Persistence.Attributes;

using Notion.Client;

namespace Norison.TransactionSync.Persistence.Storages.Models;

public class CategoryDbModel : IDbModel
{
    public string? Id { get; set; }
    public string? IconUrl { get; set; }

    [NotionProperty("Name", PropertyType.Title)]
    public string? Name { get; set; }

    [NotionProperty("Type", PropertyType.Select)]
    public CategoryType? Type { get; set; }

    [NotionProperty("Parent Category", PropertyType.Relation)]
    public string[] ParentCategoryIds { get; set; } = [];

    [NotionProperty("Sub Categories", PropertyType.Relation)]
    public string[] SubCategoryIds { get; set; } = [];
}