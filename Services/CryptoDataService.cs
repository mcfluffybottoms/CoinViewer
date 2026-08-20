using CoinViewer.Data;
using CoinViewer.DTOs;
using CoinViewer.Models;

namespace CoinViewer.Services;

public class CryptoDataService(ICoinRepository repo, GeckoAPIAccessService client)
{
    private readonly ICoinRepository _repo = repo;
    private readonly GeckoAPIAccessService _client = client;
    public List<Coin> GetCoinList()
    {
        return _repo.GetAllCoins();
    }
    public async Task<CoinReturnView> AddCoinAsync(CoinSymbol symbol)
    {
        var addedCoin = await _client.GetCoin(symbol);
        _repo.AddCoinToDb(addedCoin);
        return new(addedCoin);
    }
    public Coin GetCoinInfo(CoinSymbol symbol)
    {
        if (!_repo.TryGetCoinInfo(symbol, out Coin? coin))
        {
            throw new KeyNotFoundException($"Tracked symbol '{symbol.Symbol}' not found.");
        }

        return coin!;
    }
    public async Task<CoinReturnView> RefreshCoinPriceAsync(CoinSymbol symbol)
    {
        if (!_repo.CoinExists(symbol))
        {
            throw new KeyNotFoundException($"Tracked symbol '{symbol.Symbol}' not found.");
        }
        var newCoin = await _client.GetCoin(symbol);
        _repo.ChangeCoin(newCoin);
        return new(newCoin);
    }
    public CoinHistory GetCoinHistory(CoinSymbol symbol)
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
    public CoinInfo GetCoinStats(CoinSymbol symbol)
    {
        return null;
    }
    public void DeleteCoin(CoinSymbol symbol)
    {
        var deleted = _repo.DeleteCoinFromDb(symbol);
        if (!deleted)
        {
            throw new KeyNotFoundException($"Tracked symbol '{symbol.Symbol}' not found.");
        }
    }
}