namespace Norison.BankNotionConnector.Persistence.Storages.Budgets;

public class BudgetDbModel : IDbModel
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public decimal? Budget { get; set; }
    public string[] TransactionIds { get; set; } = [];
}