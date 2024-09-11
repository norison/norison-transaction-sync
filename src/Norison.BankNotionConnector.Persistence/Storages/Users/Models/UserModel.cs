namespace Norison.BankNotionConnector.Persistence.Storages.Users.Models;

public class UserModel
{
    public string Username { get; set; } = string.Empty;
    public string NotionToken { get; set; } = string.Empty;
    public string MonoToken { get; set; } = string.Empty;
    public string MonoAccountName { get; set; } = string.Empty;
}