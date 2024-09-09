namespace Norison.TransactionSync.Functions.Models;

public class UserSetting
{
    public string Name { get; set; } = string.Empty;
    public string NotionSecret { get; set; } = string.Empty;
    public string TransactionsTableId { get; set; } = string.Empty;
    public string AccountsTableId { get; set; } = string.Empty;
    public string MonoAccountName { get; set; } = string.Empty;
}