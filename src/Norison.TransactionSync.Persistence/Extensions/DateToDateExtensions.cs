using Notion.Client;

namespace Norison.TransactionSync.Persistence.Extensions;

public static class DateToDateExtensions
{
    public static DatePropertyValue ToDatePropertyValue(this DateTime? dateTime)
    {
        return new DatePropertyValue { Date = new Date { Start = dateTime } };
    }

    public static DateTime? ToDateTimeValue(this PropertyValue propertyValue)
    {
        if (propertyValue is not DatePropertyValue datePropertyValue)
        {
            throw new InvalidCastException("Property value is not a date.");
        }

        return datePropertyValue.Date.Start;
    }
}