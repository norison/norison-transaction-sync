namespace Norison.BankNotionConnector.Persistence.Storages.Accounts;

public class AccountDbModel : IDbModel
{
    public string? Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal InitialBalance { get; set; }
    public string CurrencyId { get; set; } = string.Empty;
    public string Group { get; set; } = string.Empty;
    public bool IsArchived { get; set; }
}