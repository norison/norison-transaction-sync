using Notion.Client;

namespace Norison.TransactionSync.Persistence.Extensions;

public static class StringToRichTextExtensions
{
    public static string? ToStringValue(this PropertyValue propertyValue)
    {
        return propertyValue switch
        {
            TitlePropertyValue title => string.Join("", title.Title.Select(x => x.PlainText)),
            RichTextPropertyValue richText => string.Join("", richText.RichText.Select(x => x.PlainText)),
            _ => string.Empty
        };
    }

    public static TitlePropertyValue ToTitlePropertyValue(this string? title)
    {
        return new TitlePropertyValue { Title = [new RichTextText { Text = new Text { Content = title ?? string.Empty } }] };
    }

    public static RichTextPropertyValue ToRichTextPropertyValue(this string? richText)
    {
        return new RichTextPropertyValue { RichText = [new RichTextText { Text = new Text { Content = richText ?? string.Empty } }] };
    }
}