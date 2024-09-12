using Mapster;

using MediatR;

using Norison.BankNotionConnector.Persistence.Storages;
using Norison.BankNotionConnector.Persistence.Storages.Users;

using Notion.Client;

namespace Norison.BankNotionConnector.Application.Features.Commands.AddUser;

public class AddUserCommandHandler(IStorageFactory storageFactory) : IRequestHandler<AddUserCommand>
{
    public async Task Handle(AddUserCommand request, CancellationToken cancellationToken)
    {
        var userStorage = storageFactory.GetUsersStorage();
        
        var newUser = request.Adapt<UserDbModel>();
        
        var parameters = new DatabasesQueryParameters { Filter = new TitleFilter("Username", request.Username) };
        var user = await userStorage.GetFirstAsync(parameters, cancellationToken);
        
        if (user is not null)
        {
            user.NotionToken = request.NotionToken;
            user.MonoToken = request.MonoToken;
            user.MonoAccountName = request.MonoAccountName;
            await userStorage.UpdateAsync(user, cancellationToken);
            return;
        }
        
        await userStorage.AddAsync(newUser, cancellationToken);
    }
}