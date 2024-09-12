using Notion.Client;

namespace Norison.TransactionSync.Persistence.Extensions;

public static class NumberToNumberExtensions
{
    public static NumberPropertyValue? ToNumberPropertyValue(this long number)
    {
        return new NumberPropertyValue { Number = number };
    }
    
    public static NumberPropertyValue ToNumberPropertyValue(this long? number)
    {
        return  new NumberPropertyValue { Number = number ?? 0 };
    }

    public static NumberPropertyValue ToNumberPropertyValue(this decimal? number)
    {
        return new NumberPropertyValue { Number = number is null ? 0 : (double)number };
    }

    public static long? ToLongValue(this PropertyValue propertyValue)
    {
        return ConvertToNumberPropertyValue<long>(propertyValue);
    }

    public static decimal? ToDecimalValue(this PropertyValue propertyValue)
    {
        return ConvertToNumberPropertyValue<decimal>(propertyValue);
    }

    private static T? ConvertToNumberPropertyValue<T>(PropertyValue propertyValue) where T : struct
    {
        if (propertyValue is not NumberPropertyValue numberPropertyValue)
        {
            throw new InvalidCastException("Property value is not a number.");
        }

        if (numberPropertyValue.Number is null)
        {
            return null;
        }

        return (T)Convert.ChangeType(numberPropertyValue.Number, typeof(T))!;
    }
}