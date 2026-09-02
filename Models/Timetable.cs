namespace CoinViewer.Models;

public record Timetable(
    bool Enabled,
    int IntervalSeconds,
    DateTime? LastUpdate,
    DateTime? NextUpdate
);