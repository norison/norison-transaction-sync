namespace Norison.BankNotionConnector.Persistence.Storages.Users;

public class UserDbModel : IDbModel
{
    public string? Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public long ChatId { get; set; }
    public string NotionToken { get; set; } = string.Empty;
    public string MonoToken { get; set; } = string.Empty;
    public string MonoAccountName { get; set; } = string.Empty;
}