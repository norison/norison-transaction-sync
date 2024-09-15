using Mediator;

using Microsoft.Extensions.Options;

using Monobank.Client;

using Norison.TransactionSync.Application.Options;
using Norison.TransactionSync.Application.Services.Users;

namespace Norison.TransactionSync.Application.Features.Commands.Enable;

public class EnableCommandHandler(
    IUsersService usersService,
    IMonobankClient monobankClient,
    IOptions<WebHookOptions> webHookOptions) : ICommandHandler<EnableCommand>
{
    public async ValueTask<Unit> Handle(EnableCommand request, CancellationToken cancellationToken)
    {
        var user = await usersService.GetUserByChatIdAsync(request.ChatId, cancellationToken);

        if (user is null)
        {
            throw new InvalidOperationException("User not found.");
        }

        var url = $"{webHookOptions.Value.WebHookBaseUrl}/monobank/{request.ChatId}";
        await monobankClient.Personal.SetWebHookAsync(url, user.MonoToken, cancellationToken);
        
        return Unit.Value;
    }
}