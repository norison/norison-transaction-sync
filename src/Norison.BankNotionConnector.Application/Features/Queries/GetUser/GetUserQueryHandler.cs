using MediatR;

using Norison.BankNotionConnector.Persistence.Storages;
using Norison.BankNotionConnector.Persistence.Storages.Users.Models;

namespace Norison.BankNotionConnector.Application.Features.Queries.GetUser;

public class GetUserQueryHandler(IStorageFactory storageFactory) : IRequestHandler<GetUserQuery, UserModel>
{
    public async Task<UserModel> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        var userStorage = storageFactory.GetUsersStorage();

        var user = await userStorage.GetUserAsync(request.Username, cancellationToken);

        if (user is null)
        {
            throw new InvalidOperationException($"User with username '{request.Username}' not found.");
        }

        return user;
    }
}