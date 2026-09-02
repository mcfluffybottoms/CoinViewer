using CoinViewer.Data;
using CoinViewer.DTOs;
using CoinViewer.Models;

namespace CoinViewer.Services;

public class SQLiteCoinRepository(AppDbContext context) : ICoinRepository
{
    public bool AddCoinToDb(Coin coin)
    {
        bool exists = context.Coins.Any(c => c.Symbol == coin.Symbol);
        if (exists)
        {
            return false;
        }
        context.Coins.Add(coin);
        AddCoinToHistoryNoSave(coin);
        context.SaveChanges();
        return true;
    }

    public bool ChangeCoin(Coin coin)
    {
        var existingCoin = context.Coins.SingleOrDefault(x => x.Symbol == coin.Symbol);
        if (existingCoin is null)
        {
            return false;
        }
        existingCoin.LastUpdated = coin.LastUpdated;
        existingCoin.Name = coin.Name;
        existingCoin.Price = coin.Price;
        AddCoinToHistoryNoSave(existingCoin);
        context.SaveChanges();
        return true;
    }

    public bool CoinExists(CoinSymbolDto symbol)
    {
        return context.Coins.Any(c => c.Symbol == symbol.Symbol);
    }

    public bool DeleteCoinFromDb(CoinSymbolDto symbol)
    {
        var coin = context.Coins.FirstOrDefault(c => c.Symbol == symbol.Symbol);

        if (coin is null)
        {
            return false;
        }

        var history = context.CoinHistory.Where(c => c.Symbol == symbol.Symbol);

        context.CoinHistory.RemoveRange(history);
        context.Coins.Remove(coin);

        context.SaveChanges();
        return true;
    }

    public List<Coin> GetAllCoins()
    {
        return [.. context.Coins];
    }

    public List<CoinHistoryEntry> GetCoinHistory(CoinSymbolDto symbol)
    {
        return [.. context.CoinHistory.Where(c => c.Symbol == symbol.Symbol)];
    }

    public bool TryGetCoinInfo(CoinSymbolDto symbol, out Coin? coin)
    {
        coin = context.Coins.FirstOrDefault(c => c.Symbol == symbol.Symbol);
        return coin != null;
    }

    private void AddCoinToHistoryNoSave(Coin coin)
    {
        context.CoinHistory.Add(new CoinHistoryEntry
        {
            Symbol = coin.Symbol,
            Price = coin.Price,
            Timestamp = coin.LastUpdated
        });
        var oldEntries = context.CoinHistory
            .Where(c => c.Symbol == coin.Symbol)
            .OrderByDescending(h => h.Timestamp)
            .Skip(100)
            .ToList();
        if (oldEntries.Count > 0)
        {
            context.CoinHistory.RemoveRange(oldEntries);
        }
    }
}