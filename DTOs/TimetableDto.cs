using System.Text.Json.Serialization;

namespace CoinViewer.DTOs;

public record TimetableDto(
    bool Enabled,
    [property: JsonPropertyName("interval_seconds")]
    int IntervalSeconds,
    [property: JsonPropertyName("last_update")]
    DateTime? LastUpdate,
    [property: JsonPropertyName("next_update")]
    DateTime? NextUpdate
);

public record TimetableChangeDto(
    bool Enabled,
    [property: JsonPropertyName("interval_seconds")]
    int IntervalSeconds
);

public record ReloadResultDto(
    [property: JsonPropertyName("updated_count")]
    int UpdatedCount,
    DateTime Timestamp
);