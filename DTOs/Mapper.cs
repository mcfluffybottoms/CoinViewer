using CoinViewer.Models;

namespace CoinViewer.DTOs;

public static class Mapper
{
    public static Coin ToModel(CoinDto obj)
    {
        return new Coin
        {
            Symbol = obj.Symbol,
            Name = obj.Name,
            Price = obj.Price,
            LastUpdated = obj.LastUpdated
        };
    }

    public static CoinDto ToDto(Coin obj)
    {
        return new CoinDto
        {
            Symbol = obj.Symbol,
            Name = obj.Name,
            Price = obj.Price,
            LastUpdated = obj.LastUpdated
        };
    }

    public static CoinHistoryEntryDto ToDto(CoinHistoryEntry obj)
    {
        return new CoinHistoryEntryDto
        {
            Price = obj.Price,
            Timestamp = obj.Timestamp
        };
    }

    public static Timetable ToModel(TimetableChangeDto obj)
    {
        return new Timetable(
            obj.Enabled,
            obj.IntervalSeconds,
            null, null
        );
    }

    public static TimetableDto ToDto(Timetable obj)
    {
        return new TimetableDto(
            obj.Enabled,
            obj.IntervalSeconds,
            obj.LastUpdate,
            obj.NextUpdate
        );
    }
}