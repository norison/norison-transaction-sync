namespace Norison.BankNotionConnector.Persistence.Storages.Transactions;

public class TransactionDbModel : IDbModel
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public TransactionType? Type { get; set; }
    public DateTime? Date { get; set; }
    public decimal? AmountFrom { get; set; }
    public decimal? AmountTo { get; set; }
    public string? Notes { get; set; }
    public string[] AccountFromIds { get; set; } = [];
    public string[] AccountToIds { get; set; } = [];
    public string[] CategoryIds { get; set; } = [];
    public string[] BudgetIds { get; set; } = [];
}