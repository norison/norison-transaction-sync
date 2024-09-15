using Norison.TransactionSync.Persistence.Storages.Models;

namespace Norison.TransactionSync.Application.Services.Users;

public interface IUsersService
{
    Task<UserDbModel[]> GetAllUsersAsync(CancellationToken cancellationToken = default);
    Task<UserDbModel?> GetUserByChatIdAsync(long chatId, CancellationToken cancellationToken = default);
    Task SetUserAsync(UserDbModel user, CancellationToken cancellationToken = default);
}