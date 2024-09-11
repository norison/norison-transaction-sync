using Norison.BankNotionConnector.Persistence.Storages.Users.Models;

namespace Norison.BankNotionConnector.Persistence.Storages.Users;

public interface IUsersStorage : IStorage
{
    Task<UserModel[]> GetUsersAsync(CancellationToken cancellationToken);
    Task<UserModel?> GetUserAsync(string username, CancellationToken cancellationToken);
    Task AddUserAsync(UserModel user, CancellationToken cancellationToken);
    Task UpdateUserAsync(UserModel user, CancellationToken cancellationToken);
}