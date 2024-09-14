using MediatR;

namespace Norison.TransactionSync.Application.Features.Commands.SetSettings;

public class SetSettingsCommand : IRequest
{
    public long ChatId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
}