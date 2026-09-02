using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoinViewer.Models;

[Table("CoinsHistory")]  
public class CoinHistoryEntry
{
    public required string Symbol { get; set; }
    public required decimal Price { get; set; }
    public required DateTime Timestamp { get; set; }
}