using System.Text.Json.Serialization;

namespace CoinViewer.DTOs;

public class CoinInfoDto
{
    public required string Symbol { get; set; }
    [JsonPropertyName("current_price")]
    public required decimal CurrentPrice { get; set; }
    public required CoinStatisticsDto Stats { get; set; }
}

public class CoinStatisticsDto
{
    [JsonPropertyName("min_price")]
    public required decimal MinPrice { get; set; }
    [JsonPropertyName("max_price")]
    public required decimal MaxPrice { get; set; }
    [JsonPropertyName("avg_price")]
    public required decimal AvgPrice { get; set; }
    [JsonPropertyName("price_change")]
    public required decimal PriceChange { get; set; }
    [JsonPropertyName("price_change_percent")]
    public required decimal PriceChangePercent { get; set; }
    [JsonPropertyName("records_count")]
    public required decimal RecordsCount { get; set; }
}