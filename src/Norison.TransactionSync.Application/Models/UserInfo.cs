namespace Norison.TransactionSync.Application.Models;

public class UserInfo
{
    public string MonoAccountBankId { get; set; } = string.Empty;
    public string MonoAccountId { get; set; } = string.Empty;
    public string AccountsDatabaseId { get; set; } = string.Empty;
    public string BudgetsDatabaseId { get; set; } = string.Empty;
    public string TransactionsDatabaseId { get; set; } = string.Empty;
    public string CategoriesDatabaseId { get; set; } = string.Empty;
    public string CurrenciesDatabaseId { get; set; } = string.Empty;
    public string AutomationsDatabaseId { get; set; } = string.Empty;
}