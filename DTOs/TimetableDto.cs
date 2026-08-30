namespace CoinViewer.DTOs;

public record TimetableDto(
    bool Enabled,
    int IntervalSeconds,
    DateTime? LastUpdate,
    DateTime? NextUpdate
);

public record TimetableChangeDto(
    bool Enabled,
    int IntervalSeconds
);

public record ReloadResultDto(
    int UpdatedCount,
    DateTime Timestamp
);