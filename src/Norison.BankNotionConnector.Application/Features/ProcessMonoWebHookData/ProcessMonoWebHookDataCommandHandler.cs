using MediatR;

using Norison.BankNotionConnector.Persistence.Storages;

using Notion.Client;

namespace Norison.BankNotionConnector.Application.Features.ProcessMonoWebHookData;

public class ProcessMonoWebHookDataCommandHandler(IStorageFactory storageFactory)
    : IRequestHandler<ProcessMonoWebHookDataCommand>
{
    public async Task Handle(ProcessMonoWebHookDataCommand request, CancellationToken cancellationToken)
    {
        var userStorage = storageFactory.GetUsersStorage();

        var parameters = new DatabasesQueryParameters { Filter = new NumberFilter("ChatId", request.ChatId) };
        var user = await userStorage.GetFirstAsync(parameters, cancellationToken);

        if (user is null)
        {
            return;
        }

        var transactionsStorage = storageFactory.GetTransactionsStorage(user.NotionToken);

        var transactions = await transactionsStorage.GetAllAsync(new DatabasesQueryParameters(), cancellationToken);
    }
}