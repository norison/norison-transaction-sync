using Norison.BankNotionConnector.Persistence.Extensions;

using Notion.Client;

namespace Norison.BankNotionConnector.Persistence.Storages.Transactions;

public class TransactionsStorage(INotionClient client) : StorageBase<TransactionDbModel>(client)
{
    protected override string DatabaseName => "Transactions";

    protected override TransactionDbModel ConvertPropertiesToModel(string id,
        IDictionary<string, PropertyValue> properties)
    {
        return new TransactionDbModel
        {
            Id = id,
            Name = properties["Name"].ToStringValue(),
            Date = properties["Date"].ToDateTimeValue(),
            Type = properties["Type"].ToEnumValue<TransactionType>(),
            AccountFromIds = properties["Account From"].ToArrayValue(),
            AccountToIds = properties["Account To"].ToArrayValue(),
            CategoryIds = properties["Category"].ToArrayValue(),
            AmountFrom = properties["Amount From"].ToDecimalValue(),
            AmountTo = properties["Amount To"].ToDecimalValue(),
            Notes = properties["Notes"].ToStringValue(),
            BudgetIds = properties["Budget"].ToArrayValue()
        };
    }

    protected override IDictionary<string, PropertyValue> ConvertModelToProperties(TransactionDbModel model)
    {
        return new Dictionary<string, PropertyValue>
        {
            { "Name", model.Name.ToTitlePropertyValue() },
            { "Date", model.Date.ToDatePropertyValue() },
            { "Type", model.Type.ToSelectPropertyValue() },
            { "Account From", model.AccountFromIds.ToRelationPropertyValue() },
            { "Account To", model.AccountToIds.ToRelationPropertyValue() },
            { "Category", model.CategoryIds.ToRelationPropertyValue() },
            { "Amount From", model.AmountFrom.ToNumberPropertyValue() },
            { "Amount To", model.AmountTo.ToNumberPropertyValue() },
            { "Notes", model.Notes.ToRichTextPropertyValue() },
            { "Budget", model.BudgetIds.ToRelationPropertyValue() }
        };
    }
}