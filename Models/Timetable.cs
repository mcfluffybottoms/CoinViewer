namespace CoinViewer.Models;

public record Timetable(
    bool Enabled,
    int IntervalSeconds,
    DateTime? LastUpdate,
    DateTime? NextUpdate
);

public record TimetableChange(
    bool Enabled,
    int IntervalSeconds
);

public record ReloadResult(
    int UpdatedCount,
    DateTime Timestamp
);