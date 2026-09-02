using CoinViewer.DTOs;
using CoinViewer.Models;

namespace CoinViewer.Data;

public interface ICoinRepository
{
    public List<Coin> GetAllCoins();
    public bool CoinExists(CoinSymbolDto symbol);
    public bool AddCoinToDb(Coin coin);
    public bool ChangeCoin(Coin coin);
    public bool DeleteCoinFromDb(CoinSymbolDto symbol);
    public bool TryGetCoinInfo(CoinSymbolDto symbol, out Coin? coin);
    public List<CoinHistoryEntry> GetCoinHistory(CoinSymbolDto coin);
} 