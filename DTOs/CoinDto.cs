using System.Text.Json.Serialization;
using CoinViewer.DTOs;

namespace CoinViewer.DTOs;

public record CoinSymbolDto(string Symbol);

public class CoinDto
{
    public required string Symbol { get; set; }
    public required string Name { get; set; }
    [JsonPropertyName("current_price")]
    public required decimal Price { get; set; }
    [JsonPropertyName("last_updated")]
    public required DateTime LastUpdated { get; set; }
}

public record CoinReturnView(CoinDto Crypto);

public record CoinsReturnView(List<CoinDto> Cryptos);