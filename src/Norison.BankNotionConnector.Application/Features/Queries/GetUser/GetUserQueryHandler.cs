using MediatR;

using Norison.BankNotionConnector.Persistence.Storages;
using Norison.BankNotionConnector.Persistence.Storages.Users;

using Notion.Client;

namespace Norison.BankNotionConnector.Application.Features.Queries.GetUser;

public class GetUserQueryHandler(IStorageFactory storageFactory) : IRequestHandler<GetUserQuery, UserDbModel>
{
    public async Task<UserDbModel> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        var usersStorage = storageFactory.GetUsersStorage();

        var parameters = new DatabasesQueryParameters { Filter = new TitleFilter("Username", request.Username) };
        var user = await usersStorage.GetFirstAsync(parameters, cancellationToken);

        if (user is null)
        {
            throw new InvalidOperationException($"User with username '{request.Username}' not found.");
        }

        return user;
    }
}