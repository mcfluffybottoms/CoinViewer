using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CoinViewer.Services;

public class PriceUpdater(IServiceScopeFactory scopeFactory) : BackgroundService
{
    private readonly SemaphoreSlim _updateLock = new(1, 1);

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        using var scope = scopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<TimetableService>();

        while (!ct.IsCancellationRequested)
        {
            var currentTimetable = service.GetTimetable();

            if (!currentTimetable.Enabled)
            {
                await service.WaitForChangeAsync(ct);
                continue;
            }

            using var timerCt = CancellationTokenSource.CreateLinkedTokenSource(ct);

            var delayTask = Task.Delay(TimeSpan.FromSeconds(currentTimetable.IntervalSeconds), timerCt.Token);
            var scheduleTask = service.WaitForChangeAsync(ct);
            Task completedTask = await Task.WhenAny(delayTask, scheduleTask);
    
            if (completedTask == scheduleTask)
            {
                timerCt.Cancel();
                continue;
            }
            await delayTask;

            if (ct.IsCancellationRequested) break;
            await UpdatePricesAsync(service, ct);
        }
    }
    private async Task UpdatePricesAsync(TimetableService service, CancellationToken ct)
    {
        await _updateLock.WaitAsync(ct);
        try
        {
            await service.UpdatePricesAsync(ct);
        }
        finally
        {
            _updateLock.Release();
        }
    }

    public override void Dispose()
    {
        _updateLock.Dispose();
        base.Dispose();
        GC.SuppressFinalize(this);
    }
}