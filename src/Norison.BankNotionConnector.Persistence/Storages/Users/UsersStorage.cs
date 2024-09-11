using Norison.BankNotionConnector.Persistence.Extensions;
using Norison.BankNotionConnector.Persistence.Storages.Users.Models;

using Notion.Client;

namespace Norison.BankNotionConnector.Persistence.Storages.Users;

public class UsersStorage(INotionClient client) : StorageBase(client), IUsersStorage
{
    private const string DatabaseName = "Users";
    private const string UsernamePropertyName = "Username";

    private readonly INotionClient _client = client;

    public async Task<string> GetDatabaseIdAsync(CancellationToken cancellationToken)
    {
        var database = await GetDatabaseAsync(DatabaseName, cancellationToken);
        return database.Id;
    }

    public async Task<UserModel[]> GetUsersAsync(CancellationToken cancellationToken)
    {
        var pages = await GetPagesAsync(new DatabasesQueryParameters(), cancellationToken);
        return pages.Select(page => page.Properties.ToUserModel()).ToArray();
    }

    public async Task<UserModel?> GetUserAsync(string username, CancellationToken cancellationToken)
    {
        var parameters = new DatabasesQueryParameters { Filter = new TitleFilter(UsernamePropertyName, username) };
        var pages = await GetPagesAsync(parameters, cancellationToken);
        return pages.FirstOrDefault()?.Properties.ToUserModel();
    }

    public async Task AddUserAsync(UserModel user, CancellationToken cancellationToken)
    {
        var databaseId = await GetDatabaseIdAsync(cancellationToken);

        var properties = user.ToProperties();

        var parameters = new PagesCreateParameters
        {
            Parent = new DatabaseParentInput { DatabaseId = databaseId }, Properties = properties
        };

        await _client.Pages.CreateAsync(parameters, cancellationToken);
    }

    public async Task UpdateUserAsync(UserModel user, CancellationToken cancellationToken)
    {
        var queryParameters =
            new DatabasesQueryParameters { Filter = new TitleFilter(UsernamePropertyName, user.Username) };

        var pages = await GetPagesAsync(queryParameters, cancellationToken);

        if (pages.Length == 0)
        {
            throw new InvalidOperationException($"User with username '{user.Username}' does not exist.");
        }

        var pageId = pages[0].Id;

        var parameters = new PagesUpdateParameters { Properties = user.ToProperties() };

        await _client.Pages.UpdateAsync(pageId, parameters, cancellationToken);
    }

    private async Task<Page[]> GetPagesAsync(DatabasesQueryParameters parameters, CancellationToken cancellationToken)
    {
        var databaseId = await GetDatabaseIdAsync(cancellationToken);
        var pages = await _client.Databases.QueryAsync(databaseId, parameters, cancellationToken);
        return pages.Results.ToArray();
    }
}