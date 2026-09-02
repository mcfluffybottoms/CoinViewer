namespace CoinViewer.DTOs;


public class CoinHistoryEntryDto
{
    public required decimal Price { get; set; }
    public required DateTime Timestamp { get; set; }
}

public class CoinHistoryDto {
    public required string Symbol { get; set; }
    public List<CoinHistoryEntryDto> History { get; set; } = [];
}