namespace Norison.TransactionSync.Application.Services.Stocks;

public interface IStocksService
{
    Task<decimal> GetSymbolPriceAsync(string symbol);
}