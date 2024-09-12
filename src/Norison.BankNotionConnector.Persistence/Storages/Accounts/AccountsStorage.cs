using Norison.BankNotionConnector.Persistence.Extensions;

using Notion.Client;

namespace Norison.BankNotionConnector.Persistence.Storages.Accounts;

public class AccountsStorage(INotionClient client) : StorageBase<AccountDbModel>(client)
{
    protected override string DatabaseName => "Accounts";

    protected override AccountDbModel ConvertPropertiesToModel(string id, IDictionary<string, PropertyValue> properties)
    {
        return new AccountDbModel { Id = id, Name = properties["Name"].ToStringValue() };
    }

    protected override IDictionary<string, PropertyValue> ConvertModelToProperties(AccountDbModel model)
    {
        return new Dictionary<string, PropertyValue> { { "Name", model.Name.ToTitlePropertyValue() } };
    }
}