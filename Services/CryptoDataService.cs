using CoinViewer.Data;
using CoinViewer.DTOs;
using CoinViewer.Models;

namespace CoinViewer.Services;

public class CryptoDataService(ICoinRepository repo, IAPIAccessService client)
{
    public enum AddResult
    {
        ADDED,
        CONFLICT,
        API_NOT_FOUND
    }
    public enum RefreshResult
    {
        REFRESHED,
        NOT_FOUND,
        API_NOT_FOUND
    }
    private readonly ICoinRepository _repo = repo;
    private readonly IAPIAccessService _client = client;
    public List<Coin> GetCoinList(CancellationToken ct = default)
    {
        return _repo.GetAllCoins();
    }
    public async Task<(AddResult result, CoinDto? addedCoin)> AddCoinAsync(CoinSymbolDto symbol, CancellationToken ct = default)
    {
        bool added = _repo.CoinExists(symbol);
        if(added)
        {
            return (AddResult.CONFLICT, null);
        }
        var addedCoin = await _client.GetCoin(symbol, ct);
        if(addedCoin is null)
        {
            return (AddResult.API_NOT_FOUND, null);
        }
        _repo.AddCoinToDb(addedCoin);
        return (AddResult.ADDED, Mapper.ToDto(addedCoin));
    }
    public CoinDto? GetCoinInfo(CoinSymbolDto symbol, CancellationToken ct = default)
    {
        if (!_repo.TryGetCoinInfo(symbol, out Coin? coin) || coin == null)
        {
            return null;
        }
        
        return new CoinDto{
            Symbol = coin!.Symbol,
            Name = coin.Name,
            Price = coin.Price,
            LastUpdated = coin.LastUpdated
        };
    }
    public async Task<(RefreshResult result, CoinReturnView? coin)> RefreshCoinPriceAsync(CoinSymbolDto symbol, CancellationToken ct = default)
    {
        bool added = _repo.CoinExists(symbol);
        if(!added)
        {
            return (RefreshResult.NOT_FOUND, null);
        }
        var newCoin = await _client.GetCoin(symbol, ct);
        if(newCoin is null)
        {
            return (RefreshResult.API_NOT_FOUND, null);
        }
        _repo.ChangeCoin(newCoin);
        return (RefreshResult.REFRESHED, new(Mapper.ToDto(newCoin)));
    }
    public CoinHistoryDto GetCoinHistory(CoinSymbolDto symbol, CancellationToken ct = default)
    {
        List<CoinHistoryEntry> history = _repo.GetCoinHistory(symbol);
        if (history.Count == 0)
        {
            return new CoinHistoryDto
            {
                Symbol = symbol.Symbol,
                History = []
            };
        }
        return new CoinHistoryDto
        {
            Symbol = symbol.Symbol,
            History = [.. history.Select(Mapper.ToDto)]
        };
    }
    public CoinInfoDto GetCoinStats(CoinSymbolDto symbol, CancellationToken ct = default)
    {
        _repo.TryGetCoinInfo(symbol, out Coin? coin);
        List<CoinHistoryEntry> history = _repo.GetCoinHistory(symbol);
        var firstPrice = history.First().Price;
        var lastPrice = history.Last().Price;
        var stats = new CoinStatisticsDto
        {
            MinPrice = coin is null ? 0 : history.MinBy(coin => coin.Price)!.Price,
            MaxPrice = coin is null ? 0 : history.MaxBy(coin => coin.Price)!.Price,
            AvgPrice = coin is null ? 0 : history.Average(coin => coin.Price),
            PriceChange = coin is null ? 0 : lastPrice - firstPrice,
            PriceChangePercent = coin is null ? 0 : (firstPrice == 0 ? 0 : (lastPrice - firstPrice) / firstPrice * 100),
            RecordsCount = history.Count
        };

        return new CoinInfoDto {
            Symbol = symbol.Symbol,
            CurrentPrice = coin is null ? 0 : coin.Price,
            Stats = stats
        };
    }
    public bool DeleteCoin(CoinSymbolDto symbol, CancellationToken ct = default)
    {
        return _repo.DeleteCoinFromDb(symbol);
    }
}