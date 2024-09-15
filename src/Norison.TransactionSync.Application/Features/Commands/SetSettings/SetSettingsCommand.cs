using Mediator;

namespace Norison.TransactionSync.Application.Features.Commands.SetSettings;

public class SetSettingsCommand : ICommand
{
    public long ChatId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
}