using Norison.TransactionSync.Functions.Models;

namespace Norison.TransactionSync.Functions.Options;

public class UsersOptions
{
    public UserSetting[] UserSettings { get; set; } = Array.Empty<UserSetting>();
}