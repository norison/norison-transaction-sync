using Notion.Client;

namespace Norison.TransactionSync.Persistence.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class NotionPropertyAttribute(string name, PropertyType type) : Attribute
{
    public string Name { get; } = name;
    public PropertyType Type { get; } = type;
}