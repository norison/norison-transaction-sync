using MediatR;

namespace Norison.BankNotionConnector.Application.Features.Commands.AddUser;

public class AddUserCommand : IRequest
{
    public string Username { get; set; } = string.Empty;
    public long ChatId { get; set; }
    public string NotionToken { get; set; } = string.Empty;
    public string MonoToken { get; set; } = string.Empty;
    public string MonoAccountName { get; set; } = string.Empty;
}