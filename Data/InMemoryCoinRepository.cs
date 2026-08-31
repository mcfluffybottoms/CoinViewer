using CoinViewer.DTOs;
using CoinViewer.Models;

namespace CoinViewer.Data;

public class InMemoryCoinRepository : ICoinRepository
{
    private readonly Dictionary<string, Coin> coins = [];
    private readonly Dictionary<string, Queue<CoinHistoryEntry>> coinHistory = [];
    private const int LIMIT = 100;
    public bool CoinExists(CoinSymbolDto symbol)
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
        coinHistory[coin.Symbol] = [];
        AddCoinHistoryEntry(coin);
        return true;
    }
    public bool ChangeCoin(Coin coin)
    {
        if (!coins.ContainsKey(coin.Symbol))
        {
            return false;
        }
        coins[coin.Symbol] = coin;
        coinHistory[coin.Symbol].Enqueue(new CoinHistoryEntry
        {
            Price = coin.Price,
            Timestamp = coin.LastUpdated
        });
        return true;
    }
    public bool DeleteCoinFromDb(CoinSymbolDto symbol)
    {
        if (!coins.ContainsKey(symbol.Symbol))
        {
            return false;
        }
        coins.Remove(symbol.Symbol);
        coinHistory.Remove(symbol.Symbol);
        return true;
    }
    public List<CoinHistoryEntry> GetCoinHistory(CoinSymbolDto symbol)
    {
        if (coinHistory.TryGetValue(symbol.Symbol, out var history))
        {
            return [.. history.TakeLast(LIMIT)];
        }

        return [];
    }
    public bool TryGetCoinInfo(CoinSymbolDto symbol, out Coin? coin)
    {
        return coins.TryGetValue(symbol.Symbol, out coin);
    }

    private void AddCoinHistoryEntry(Coin coin)
    {
        coinHistory[coin.Symbol].Enqueue(new CoinHistoryEntry
        {
            Price = coin.Price,
            Timestamp = coin.LastUpdated
        });
        if(coinHistory[coin.Symbol].Count > LIMIT)
        {
            coinHistory[coin.Symbol].Dequeue();
        }
    }
} 