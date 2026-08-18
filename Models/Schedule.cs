namespace CoinViewer.Models;

public class Schedule
{
    public required bool Enabled { get; set; }
    public int IntervalSeconds { get; set; }
    public DateTime LastUpdate { get; set; }
    public DateTime NextUpdate { get; set; }
}

public class ScheduleChange
{
    public required bool Enabled { get; set; }
    public int IntervalSeconds { get; set; }
}

public class ForceLoad
{
    public required decimal UpdatedCount { get; set; }
    public DateTime Timestamp { get; set; }
}