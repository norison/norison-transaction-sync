using System.Reflection;

using Microsoft.Extensions.Caching.Memory;

using Norison.TransactionSync.Persistence.Attributes;

using Notion.Client;

namespace Norison.TransactionSync.Persistence.Storages;

public class Storage<T>(INotionClient client, IMemoryCache memoryCache, string databaseName)
    : IStorage<T> where T : IDbModel
{
    public async Task<string> GetDatabaseIdAsync(CancellationToken cancellationToken)
    {
        if (memoryCache.TryGetValue(databaseName, out string? databaseId))
        {
            return databaseId!;
        }

        var database = await GetDatabaseAsync(cancellationToken);
        return database.Id;
    }

    public async Task<T[]> GetAllAsync(DatabasesQueryParameters parameters, CancellationToken cancellationToken)
    {
        var databaseId = await GetDatabaseIdAsync(cancellationToken);
        var pages = await client.Databases.QueryAsync(databaseId, parameters, cancellationToken);
        return pages.Results.Select(x => ConvertPropertiesToModel(x.Id, x.Properties)).ToArray();
    }

    public async Task<T?> GetFirstAsync(DatabasesQueryParameters parameters, CancellationToken cancellationToken)
    {
        var databaseId = await GetDatabaseIdAsync(cancellationToken);
        var pages = await client.Databases.QueryAsync(databaseId, parameters, cancellationToken);
        var page = pages.Results.FirstOrDefault();
        return page is not null ? ConvertPropertiesToModel(page.Id, page.Properties) : default;
    }

    public async Task AddAsync(T item, CancellationToken cancellationToken)
    {
        var databaseId = await GetDatabaseIdAsync(cancellationToken);
        var parameters = new PagesCreateParameters
        {
            Parent = new DatabaseParentInput { DatabaseId = databaseId },
            Properties = ConvertModelToProperties(item)
        };
        await client.Pages.CreateAsync(parameters, cancellationToken);
    }

    public async Task UpdateAsync(T item, CancellationToken cancellationToken)
    {
        var parameters = new PagesUpdateParameters { Properties = ConvertModelToProperties(item) };
        await client.Pages.UpdateAsync(item.Id, parameters, cancellationToken);
    }

    private async Task<Database> GetDatabaseAsync(CancellationToken cancellationToken)
    {
        var parameters = new SearchParameters
        {
            Query = databaseName, Filter = new SearchFilter { Value = SearchObjectType.Database }
        };

        var databases = await client.Search.SearchAsync(parameters, cancellationToken);

        var database = databases.Results.FirstOrDefault();

        if (database is null)
        {
            throw new InvalidOperationException($"Database '{databaseName}' not found.");
        }

        return (Database)database;
    }

    private static Dictionary<string, PropertyValue> ConvertModelToProperties(T model)
    {
        var properties = typeof(T).GetProperties();

        var result = new Dictionary<string, PropertyValue>();

        foreach (var property in properties)
        {
            var attribute = property.GetCustomAttribute<NotionPropertyAttribute>();

            if (attribute is null)
            {
                continue;
            }

            var value = property.GetValue(model);

            var propertyValue = ConvertValueToPropertyValue(attribute.Type, value);

            if (propertyValue is not null)
            {
                result.Add(attribute.Name, propertyValue);
            }
        }

        return result;
    }

    private static T ConvertPropertiesToModel(string id, IDictionary<string, PropertyValue> properties)
    {
        var model = Activator.CreateInstance<T>();

        var modelProperties = typeof(T).GetProperties();

        foreach (var property in modelProperties)
        {
            var attribute = property.GetCustomAttribute<NotionPropertyAttribute>();

            if (attribute is null)
            {
                continue;
            }

            if (!properties.TryGetValue(attribute.Name, out var propertyValue))
            {
                continue;
            }

            var value = ConvertPropertyValueToValue(attribute.Type, propertyValue);
            var targetType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
            var convertedValue = value is null ? null : Convert.ChangeType(value, targetType);
            property.SetValue(model, convertedValue);
        }

        model.Id = id;

        return model;
    }

    private static object? ConvertPropertyValueToValue(PropertyType type, PropertyValue propertyValue)
    {
        return type switch
        {
            PropertyType.Title => (propertyValue as TitlePropertyValue)?.Title?.FirstOrDefault()?.PlainText,
            PropertyType.RichText => (propertyValue as RichTextPropertyValue)?.RichText?.FirstOrDefault()?.PlainText,
            PropertyType.Number => (propertyValue as NumberPropertyValue)?.Number,
            PropertyType.Checkbox => (propertyValue as CheckboxPropertyValue)?.Checkbox,
            PropertyType.Date => (propertyValue as DatePropertyValue)?.Date?.Start,
            PropertyType.Select => (propertyValue as SelectPropertyValue)?.Select?.Name,
            PropertyType.Relation => (propertyValue as RelationPropertyValue)?.Relation?.Select(x => x.Id).ToArray(),
            _ => throw new NotSupportedException($"Property type '{type}' is not supported.")
        };
    }

    private static PropertyValue? ConvertValueToPropertyValue(PropertyType type, object? value)
    {
        return type switch
        {
            PropertyType.Title => new TitlePropertyValue
            {
                Title = [new RichTextText { Text = new Text { Content = value?.ToString() ?? string.Empty } }]
            },
            PropertyType.RichText => new RichTextPropertyValue
            {
                RichText = [new RichTextText { Text = new Text { Content = value?.ToString() ?? string.Empty } }]
            },
            PropertyType.Number => value is null
                ? null
                : new NumberPropertyValue { Number = double.Parse(value.ToString()!) },
            PropertyType.Checkbox => new CheckboxPropertyValue { Checkbox = value is not null && (bool)value },
            PropertyType.Date => new DatePropertyValue { Date = new Date { Start = (DateTime?)value } },
            PropertyType.Select => new SelectPropertyValue
            {
                Select = value is null ? null : new SelectOption { Name = value.ToString() }
            },
            PropertyType.Relation => new RelationPropertyValue
            {
                Relation = value is null
                    ? []
                    : (value as IEnumerable<string>)!.Select(x => new ObjectId { Id = x }).ToList()
            },
            _ => throw new NotSupportedException($"Property type '{type}' is not supported.")
        };
    }
}