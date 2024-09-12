using MediatR;

namespace Norison.BankNotionConnector.Application.Features.GetSettings;

public class GetSettingsCommand : IRequest
{
    public string Username { get; set; } = string.Empty;
    public long ChatId { get; set; }
}