using Norison.BankNotionConnector.Functions.Models;

namespace Norison.BankNotionConnector.Functions.Options;

public class UsersOptions
{
    public UserSetting[] UserSettings { get; set; } = Array.Empty<UserSetting>();
}