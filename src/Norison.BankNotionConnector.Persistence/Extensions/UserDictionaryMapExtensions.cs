using Norison.BankNotionConnector.Persistence.Storages.Users.Models;

using Notion.Client;

namespace Norison.BankNotionConnector.Persistence.Extensions;

public static class UserDictionaryMapExtensions
{
    public static UserModel ToUserModel(this IDictionary<string, PropertyValue> properties)
    {
        return new UserModel
        {
            Username = properties["Username"].ToStringValue(),
            NotionToken = properties["NotionToken"].ToStringValue(),
            MonoToken = properties["MonoToken"].ToStringValue(),
            MonoAccountName = properties["MonoAccountName"].ToStringValue()
        };
    }

    public static IDictionary<string, PropertyValue> ToProperties(this UserModel user)
    {
        return new Dictionary<string, PropertyValue>
        {
            { "Username", user.Username.ToTitlePropertyValue() },
            { "NotionToken", user.NotionToken.ToRichTextPropertyValue() },
            { "MonoToken", user.MonoToken.ToRichTextPropertyValue() },
            { "MonoAccountName", user.MonoAccountName.ToRichTextPropertyValue() }
        };
    }
}