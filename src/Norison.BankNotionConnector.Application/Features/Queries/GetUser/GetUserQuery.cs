using MediatR;

using Norison.BankNotionConnector.Persistence.Storages.Users.Models;

namespace Norison.BankNotionConnector.Application.Features.Queries.GetUser;

public class GetUserQuery : IRequest<UserModel>
{
    public string Username { get; set; } = string.Empty;
}