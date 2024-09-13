using Norison.TransactionSync.Application.Models;
using Norison.TransactionSync.Persistence.Storages.Models;

namespace Norison.TransactionSync.Application.Services.UserInfos;

public interface IUserInfosService
{
    Task PopulateUserAsync(
        long chatId,
        string username,
        string monoToken,
        string notionToken,
        string monoAccountName,
        CancellationToken cancellationToken);

    Task RefreshUserAsync(long chatId, CancellationToken cancellationToken);

    Task<UserDbModel?> GetUserAsync(long chatId, CancellationToken cancellationToken);
}