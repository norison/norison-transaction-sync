using AlphaVantage.Net.Stocks.Client;

namespace Norison.TransactionSync.Application.Services.Stocks;

public class StocksService(StocksClient client) : IStocksService
{
    public async Task<decimal> GetSymbolPriceAsync(string symbol)
    {
        var quote = await client.GetGlobalQuoteAsync(symbol);

        if (quote is null)
        {
            throw new ArgumentException($"Symbol {symbol} not found.");
        }

        return quote.Price;
    }
}