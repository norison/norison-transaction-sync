using Norison.BankNotionConnector.Persistence.Extensions;

using Notion.Client;

namespace Norison.BankNotionConnector.Persistence.Storages.Budgets;

public class BudgetsStorage(INotionClient client) : StorageBase<BudgetDbModel>(client)
{
    protected override string DatabaseName => "Budgets";

    protected override BudgetDbModel ConvertPropertiesToModel(string id, IDictionary<string, PropertyValue> properties)
    {
        return new BudgetDbModel
        {
            Id = id,
            Name = properties["Name"].ToStringValue(),
            Budget = properties["Budget"].ToDecimalValue(),
            TransactionIds = properties["Transactions"].ToArrayValue()
        };
    }

    protected override IDictionary<string, PropertyValue> ConvertModelToProperties(BudgetDbModel model)
    {
        return new Dictionary<string, PropertyValue>
        {
            { "Name", model.Name.ToTitlePropertyValue() },
            { "Budget", model.Budget.ToNumberPropertyValue() },
            { "Transactions", model.TransactionIds.ToRelationPropertyValue() }
        };
    }
}