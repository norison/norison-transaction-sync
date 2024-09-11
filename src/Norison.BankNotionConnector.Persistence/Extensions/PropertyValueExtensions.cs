using Notion.Client;

namespace Norison.BankNotionConnector.Persistence.Extensions;

public static class PropertyValueExtensions
{
    public static string ToStringValue(this PropertyValue propertyValue)
    {
        return propertyValue switch
        {
            TitlePropertyValue title => string.Join("", title.Title.Select(x => x.PlainText)),
            RichTextPropertyValue richText => string.Join("", richText.RichText.Select(x => x.PlainText)),
            _ => string.Empty
        };
    }
}