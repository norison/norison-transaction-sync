using Notion.Client;

namespace Norison.TransactionSync.Persistence.Extensions;

public static class EnumToSelectExtensions
{
    public static SelectPropertyValue ToSelectPropertyValue(this Enum? @enum)
    {
        return new SelectPropertyValue { Select = @enum is null ? null : new SelectOption { Name = @enum.ToString() } };
    }

    public static T? ToEnumValue<T>(this PropertyValue propertyValue) where T : struct
    {
        if (propertyValue is not SelectPropertyValue selectPropertyValue)
        {
            throw new InvalidCastException("Property value is not a select.");
        }

        if (selectPropertyValue.Select is null)
        {
            return null;
        }

        return (T)Enum.Parse(typeof(T), selectPropertyValue.Select.Name);
    }
}