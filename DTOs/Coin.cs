namespace CoinViewer.DTOs;

public class Coin
{
    public required string Symbol { get; set; }
    public required string Name { get; set; }
    public required decimal Price { get; set; }
    public required DateTime LastUpdated { get; set; }
}
