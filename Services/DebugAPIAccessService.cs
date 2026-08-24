using CoinViewer.DTOs;
using CoinViewer.Models;

namespace CoinViewer.Services;

public class DebugAPIAccessService : IAPIAccessService
{
    public Task<Coin> GetCoin(CoinSymbol symbol, CancellationToken ct = default)
    {
        decimal price = (decimal)Random.Shared.NextDouble() * 100_000m;
        return Task.FromResult(new Coin
        {
            Symbol = symbol.Symbol,
            Name = symbol.Symbol + "Coin",
            Price = Math.Round(price, 2),
            LastUpdated = DateTime.Now,
        });
    }
}