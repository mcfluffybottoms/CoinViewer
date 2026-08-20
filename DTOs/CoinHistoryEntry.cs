namespace CoinViewer.DTOs;

public class CoinHistoryEntry
{
    public required decimal Price { get; set; }
    public required DateTime Timestamp { get; set; }
}
