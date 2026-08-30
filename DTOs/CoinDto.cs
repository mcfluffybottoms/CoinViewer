using CoinViewer.DTOs;

namespace CoinViewer.DTOs;

public record CoinSymbolDto(string Symbol);

public class CoinDto
{
    public required string Symbol { get; set; }
    public required string Name { get; set; }
    public required decimal Price { get; set; }
    public required DateTime LastUpdated { get; set; }
}

public record CoinReturnView(CoinDto Coin);