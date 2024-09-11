using Mapster;

using MediatR;

using Norison.BankNotionConnector.Persistence.Storages;
using Norison.BankNotionConnector.Persistence.Storages.Users.Models;

namespace Norison.BankNotionConnector.Application.Features.Commands.SetUser;

public class SerUserCommandHandler(IStorageFactory storageFactory) : IRequestHandler<SetUserCommand>
{
    public async Task Handle(SetUserCommand request, CancellationToken cancellationToken)
    {
        var userStorage = storageFactory.GetUsersStorage();
        
        var newUser = request.Adapt<UserModel>();
        
        var user = await userStorage.GetUserAsync(request.Username, cancellationToken);
        
        if (user is not null)
        {
            await userStorage.UpdateUserAsync(newUser, cancellationToken);
            return;
        }
        
        await userStorage.AddUserAsync(newUser, cancellationToken);
    }
}