using Notion.Client;

namespace Norison.BankNotionConnector.Persistence.Extensions;

public static class StringExtensions
{
    public static TitlePropertyValue ToTitlePropertyValue(this string title)
    {
        return new TitlePropertyValue { Title = [new RichTextText { Text = new Text { Content = title } }] };
    }

    public static RichTextPropertyValue ToRichTextPropertyValue(this string richText)
    {
        return new RichTextPropertyValue { RichText = [new RichTextText { Text = new Text { Content = richText } }] };
    }
}