using MediatR;

namespace Norison.BankNotionConnector.Application.Features.Commands.SetUser;

public class SetUserCommand : IRequest
{
    public string Username { get; set; } = string.Empty;
    public string NotionToken { get; set; } = string.Empty;
    public string MonoToken { get; set; } = string.Empty;
    public string MonoAccountName { get; set; } = string.Empty;
}