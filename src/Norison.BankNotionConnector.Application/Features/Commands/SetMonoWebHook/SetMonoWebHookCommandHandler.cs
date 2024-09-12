using MediatR;

using Monobank.Client;

using Norison.BankNotionConnector.Persistence.Storages;

using Notion.Client;

namespace Norison.BankNotionConnector.Application.Features.Commands.SetMonoWebHook;

public class SetMonoWebHookCommandHandler(IStorageFactory storageFactory, IMonobankClient monobankClient)
    : IRequestHandler<SetMonoWebHookCommand>
{
    public async Task Handle(SetMonoWebHookCommand request, CancellationToken cancellationToken)
    {
        var userStorage = storageFactory.GetUsersStorage();

        var parameters = new DatabasesQueryParameters { Filter = new TitleFilter("Username", request.Username) };
        var user = await userStorage.GetFirstAsync(parameters, cancellationToken);

        if (user is null)
        {
            throw new InvalidOperationException($"User with username '{request.Username}' not found.");
        }

        var url = $"https://profound-roughly-goshawk.ngrok-free.app/api/monobank/{request.Username}";

        await monobankClient.Personal.SetWebHookAsync(url, user.MonoToken, cancellationToken);
    }
}