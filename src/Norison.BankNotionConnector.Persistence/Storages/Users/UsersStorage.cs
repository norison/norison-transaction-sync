using Norison.BankNotionConnector.Persistence.Extensions;

using Notion.Client;

namespace Norison.BankNotionConnector.Persistence.Storages.Users;

public class UsersStorage(INotionClient client) : StorageBase<UserDbModel>(client)
{
    protected override string DatabaseName => "Users";

    protected override UserDbModel ConvertPropertiesToModel(string id, IDictionary<string, PropertyValue> properties)
    {
        return new UserDbModel
        {
            Id = id,
            Username = properties["Username"].ToStringValue(),
            ChatId = properties["ChatId"].ToLongValue(),
            NotionToken = properties["NotionToken"].ToStringValue(),
            MonoToken = properties["MonoToken"].ToStringValue(),
            MonoAccountName = properties["MonoAccountName"].ToStringValue()
        };
    }

    protected override IDictionary<string, PropertyValue> ConvertModelToProperties(UserDbModel model)
    {
        return new Dictionary<string, PropertyValue>
        {
            { "Username", model.Username.ToTitlePropertyValue() },
            { "ChatId", model.ChatId.ToNumberPropertyValue() },
            { "NotionToken", model.NotionToken.ToRichTextPropertyValue() },
            { "MonoToken", model.MonoToken.ToRichTextPropertyValue() },
            { "MonoAccountName", model.MonoAccountName.ToRichTextPropertyValue() }
        };
    }
}