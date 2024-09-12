using Notion.Client;

namespace Norison.TransactionSync.Persistence.Extensions;

public static class BooleanToCheckboxExtensions
{
    public static CheckboxPropertyValue ToCheckboxPropertyValue(this bool boolean)
    {
        return new CheckboxPropertyValue { Checkbox = boolean };
    }

    public static bool ToBooleanValue(this PropertyValue propertyValue)
    {
        if (propertyValue is not CheckboxPropertyValue checkboxPropertyValue)
        {
            throw new InvalidCastException("Property value is not a checkbox.");
        }
        
        return checkboxPropertyValue.Checkbox;
    }
}