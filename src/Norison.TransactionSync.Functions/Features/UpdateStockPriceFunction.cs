using Mediator;

using Microsoft.Azure.Functions.Worker;

using Norison.TransactionSync.Application.Features.Commands.UpdateStockPrice;

namespace Norison.TransactionSync.Functions.Features;

public class UpdateStockPriceFunction(ISender sender)
{
    [Function(nameof(UpdateStockPriceFunction))]
    public async Task RunAsync([TimerTrigger("0 0 * * *")] TimerInfo timerInfo, CancellationToken cancellationToken)
    {
        await sender.Send(new UpdateStockPriceCommand(), cancellationToken);
    }
}