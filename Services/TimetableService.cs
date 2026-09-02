using System.Data;
using CoinViewer.DTOs;
using CoinViewer.Models;
using Microsoft.Extensions.DependencyInjection;

namespace CoinViewer.Services;

public class TimetableService(IServiceScopeFactory scopeFactory)
{
    Timetable Timetable = new(true, 30, DateTime.UtcNow, DateTime.UtcNow.AddSeconds(30));
    private TaskCompletionSource _scheduleChanged = new(TaskCreationOptions.RunContinuationsAsynchronously);

    private record Timestamp(
        DateTime LastUpdate,
        DateTime NextUpdate
    );

    private Timestamp UpdateTimeAll()
    {
        var currentTimetable = Volatile.Read(ref Timetable);
        var timestamp = new Timestamp(DateTime.UtcNow, DateTime.UtcNow.AddSeconds(currentTimetable.IntervalSeconds));
        var updatedTimetable = currentTimetable with
        {
            LastUpdate = timestamp.LastUpdate,
            NextUpdate = timestamp.NextUpdate
        };
        Interlocked.Exchange(ref Timetable, updatedTimetable);
        return timestamp;
    }

    public async Task<ReloadResultDto> UpdatePricesAsync(CancellationToken ct)
    {
        using var scope = scopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<CryptoDataService>();

        var coins = service.GetCoinList(ct);
        var tasks = coins.Select(coin => service.RefreshCoinPriceAsync(new CoinSymbolDto(coin.Symbol), ct));
        await Task.WhenAll(tasks);

        var timestamp = UpdateTimeAll();
        return new ReloadResultDto(coins.Count, timestamp.LastUpdate);
    }

    public TimetableChangeDto? UpdateTimetable(TimetableChangeDto change)
    {
        if (change.IntervalSeconds > 3600 || change.IntervalSeconds < 10)
        {
            return null;   
        }

        var currentTimetable = Volatile.Read(ref Timetable);
        var newTimetable = new Timetable(
            change.Enabled,
            change.IntervalSeconds,
            currentTimetable.LastUpdate,
            DateTime.UtcNow.AddSeconds(change.IntervalSeconds)
        );
        Interlocked.Exchange(ref Timetable, newTimetable);

        var oldSignal = Interlocked.Exchange(
            ref _scheduleChanged, new(TaskCreationOptions.RunContinuationsAsynchronously)
        );

        oldSignal.TrySetResult();
        return new TimetableChangeDto(change.Enabled, change.IntervalSeconds);
    }

    public Timetable GetTimetable()
    {
        return Volatile.Read(ref Timetable);
    }

    public Task<ReloadResultDto> TriggerAsync(CancellationToken ct)
    {
        return UpdatePricesAsync(ct);
    }

    public Task WaitForChangeAsync(CancellationToken ct)
    {
        return Volatile.Read(ref _scheduleChanged).Task.WaitAsync(ct);
    }
}