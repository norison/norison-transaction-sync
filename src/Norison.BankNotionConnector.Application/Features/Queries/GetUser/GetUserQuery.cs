using MediatR;

using Norison.BankNotionConnector.Persistence.Storages.Users;

namespace Norison.BankNotionConnector.Application.Features.Queries.GetUser;

public class GetUserQuery : IRequest<UserDbModel>
{
    public string Username { get; set; } = string.Empty;
}