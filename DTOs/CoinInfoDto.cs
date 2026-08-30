namespace CoinViewer.DTOs;

public class CoinInfoDto
{
    public required string Symbol { get; set; }
    public required decimal CurrentPrice { get; set; }
    public required CoinStatisticsDto Stats { get; set; }
}

public class CoinStatisticsDto
{
    public required decimal MinPrice { get; set; }
    public required decimal MaxPrice { get; set; }
    public required decimal AvgPrice { get; set; }
    public required decimal PriceChange { get; set; }
    public required decimal PriceChangePercent { get; set; }
    public required decimal RecordsCount { get; set; }
}