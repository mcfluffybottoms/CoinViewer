using CoinViewer.DTOs;
using CoinViewer.Models;

namespace CoinViewer.Data;

public interface ICoinDatabase
{
    public bool AddCoinToDb(Coin coin);
    public bool DeleteCoinFromDb();
    public Coin GetCoinInfoFromDb(CoinSymbol coin);
} 