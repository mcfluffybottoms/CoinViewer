using CoinViewer.Data;
using CoinViewer.DTOs;
using CoinViewer.Models;

namespace CoinViewer.Services;

public class CryptoDataService(ICoinRepository repo, IAPIAccessService client)
{
    private readonly ICoinRepository _repo = repo;
    private readonly IAPIAccessService _client = client;
    public List<Coin> GetCoinList(CancellationToken ct = default)
    {
        return _repo.GetAllCoins();
    }
    public async Task<CoinReturnView> AddCoinAsync(CoinSymbol symbol, CancellationToken ct = default)
    {
        var addedCoin = await _client.GetCoin(symbol, ct);
        _repo.AddCoinToDb(addedCoin);
        return new(addedCoin);
    }
    public Coin GetCoinInfo(CoinSymbol symbol, CancellationToken ct = default)
    {
        if (!_repo.TryGetCoinInfo(symbol, out Coin? coin))
        {
            throw new KeyNotFoundException($"Tracked symbol '{symbol.Symbol}' not found.");
        }

        return coin!;
    }
    public async Task<CoinReturnView> RefreshCoinPriceAsync(CoinSymbol symbol, CancellationToken ct = default)
    {
        if (!_repo.CoinExists(symbol))
        {
            throw new KeyNotFoundException($"Tracked symbol '{symbol.Symbol}' not found.");
        }
        var newCoin = await _client.GetCoin(symbol, ct);
        _repo.ChangeCoin(newCoin);
        return new(newCoin);
    }
    public CoinHistory GetCoinHistory(CoinSymbol symbol, CancellationToken ct = default)
    {
        List<CoinHistoryEntry> history = _repo.GetCoinHistory(symbol);
        if (history.Count == 0)
        {
            throw new KeyNotFoundException($"Tracked symbol '{symbol.Symbol}' not found.");
        }
        return new CoinHistory
        {
            Symbol = symbol.Symbol,
            History = history
        };
    }
    public CoinInfo GetCoinStats(CoinSymbol symbol, CancellationToken ct = default)
    {
        return null;
    }
    public void DeleteCoin(CoinSymbol symbol, CancellationToken ct = default)
    {
        var deleted = _repo.DeleteCoinFromDb(symbol);
        if (!deleted)
        {
            throw new KeyNotFoundException($"Tracked symbol '{symbol.Symbol}' not found.");
        }
    }
}