using Mediator;

using Microsoft.Azure.Functions.Worker;

using Norison.TransactionSync.Application.Features.Commands.UpdateCurrencyRates;

namespace Norison.TransactionSync.Functions.Features;

public class UpdateCurrencyRatesFunction(ISender sender)
{
    [Function(nameof(UpdateCurrencyRatesFunction))]
    public async Task RunAsync([TimerTrigger("0 0 * * *")] TimerInfo timerInfo)
    {
        await sender.Send(new UpdateCurrencyRatesCommand());
    }
}