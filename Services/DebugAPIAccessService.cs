using CoinViewer.DTOs;
using CoinViewer.Models;

namespace CoinViewer.Services;

public class DebugAPIAccessService : IAPIAccessService
{
    public Task<Coin?> GetCoin(CoinSymbolDto symbol, CancellationToken ct = default)
    {
        if (symbol.Symbol == "empty")
        {
            return Task.FromResult<Coin?>(null);
        }

        decimal price = (decimal)Random.Shared.NextDouble() * 100_000m;

        return Task.FromResult<Coin?>(new Coin
        {
            Symbol = symbol.Symbol,
            Name = symbol.Symbol + "Coin",
            Price = Math.Round(price, 2),
            LastUpdated = DateTime.Now
        });
    }
}