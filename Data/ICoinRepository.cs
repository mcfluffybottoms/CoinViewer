using CoinViewer.DTOs;

namespace CoinViewer.Data;

public interface ICoinRepository
{
    public List<Coin> GetAllCoins();
    public bool CoinExists(Models.CoinSymbol symbol);
    public bool AddCoinToDb(Coin coin);
    public bool ChangeCoin(Coin coin);
    public bool DeleteCoinFromDb(Models.CoinSymbol symbol);
    public bool TryGetCoinInfo(Models.CoinSymbol symbol, out Coin? coin);
    public List<CoinHistoryEntry> GetCoinHistory(Models.CoinSymbol coin);
} 