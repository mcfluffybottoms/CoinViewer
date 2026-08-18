namespace CoinViewer.Models;


public record CoinSymbol(string Symbol);

public class CoinInfo
{
    public required string Symbol { get; set; }
    public required decimal CurrentPrice { get; set; }
    public required CoinStatistics Stats { get; set; }
}

public class CoinStatistics
{
    public required decimal MinPrice { get; set; }
    public required decimal MaxPrice { get; set; }
    public required decimal AvgPrice { get; set; }
    public required decimal PriceChange { get; set; }
    public required decimal PriceChangePercent { get; set; }
    public required decimal RecordsCount { get; set; }
}