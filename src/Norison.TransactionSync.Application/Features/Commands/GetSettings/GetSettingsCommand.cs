using MediatR;

namespace Norison.TransactionSync.Application.Features.Commands.GetSettings;

public class GetSettingsCommand : IRequest
{
    public string Username { get; set; } = string.Empty;
    public long ChatId { get; set; }
}