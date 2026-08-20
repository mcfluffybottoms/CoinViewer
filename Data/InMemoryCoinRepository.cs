using CoinViewer.DTOs;

namespace CoinViewer.Data;

public class InMemoryCoinRepository : ICoinRepository
{
    private readonly Dictionary<string, Coin> coins = [];
    private readonly Dictionary<string, List<CoinHistoryEntry>> coinHistory = [];
    public bool CoinExists(Models.CoinSymbol symbol)
    {
        return coins.ContainsKey(symbol.Symbol);
    }
    public List<Coin> GetAllCoins()
    {
        return [.. coins.Values];
    }
    public bool AddCoinToDb(Coin coin)
    {
        if (coins.ContainsKey(coin.Symbol))
        {
            return false;
        }
        coins.Add(coin.Symbol, coin);
        coinHistory[coin.Symbol] = [
            new CoinHistoryEntry
            {
                Price = coin.Price,
                Timestamp = coin.LastUpdated
            }
        ];
        return true;
    }
    public bool ChangeCoin(Coin coin)
    {
        if (!coins.ContainsKey(coin.Symbol))
        {
            return false;
        }
        coins[coin.Symbol] = coin;
        coinHistory[coin.Symbol].Add(new CoinHistoryEntry
        {
            Price = coin.Price,
            Timestamp = coin.LastUpdated
        });
        return true;
    }
    public bool DeleteCoinFromDb(Models.CoinSymbol symbol)
    {
        if (!coins.ContainsKey(symbol.Symbol))
        {
            return false;
        }
        coins.Remove(symbol.Symbol);
        coinHistory.Remove(symbol.Symbol);
        return true;
    }
    public List<CoinHistoryEntry> GetCoinHistory(Models.CoinSymbol symbol)
    {
        if (coinHistory.TryGetValue(symbol.Symbol, out var history))
        {
            return history;
        }

        return [];
    }
    public bool TryGetCoinInfo(Models.CoinSymbol symbol, out Coin? coin)
    {
        return coins.TryGetValue(symbol.Symbol, out coin);
    }
} 