using MediatR;

using Norison.BankNotionConnector.Persistence.Storages;

using Notion.Client;

namespace Norison.BankNotionConnector.Application.Features.Queries.Test;

public class TestQueryHandler(IStorageFactory storageFactory) : IRequestHandler<TestQuery>
{
    public async Task Handle(TestQuery request, CancellationToken cancellationToken)
    {
        var usersStorage = storageFactory.GetUsersStorage();
        
        var parameters = new DatabasesQueryParameters { Filter = new TitleFilter("Username", "n0rison") };
        var user = await usersStorage.GetFirstAsync(parameters, cancellationToken);
        
        if (user is null)
        {
            throw new InvalidOperationException("User with username 'n0rison' not found.");
        }
        
        var accountsStorage = storageFactory.GetAccountsStorage(user.NotionToken);

        var accounts = await accountsStorage.GetAllAsync(new DatabasesQueryParameters(), cancellationToken);
        
        foreach (var account in accounts)
        {
            Console.WriteLine($"Name: {account.Name}, Balance: {account.InitialBalance}");
        }
    }
}