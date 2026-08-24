using CoinViewer.Models;
using CoinViewer.Services;
using Microsoft.AspNetCore.Mvc;

namespace CoinViewer.Controllers;

[ApiController]
[Route("schedule")]
public class TimetableController(TimetableService service) : ControllerBase
{
    [HttpGet]
    public IActionResult GetTimetable()
    {
        return Ok(service.GetTimetable());
    }

    [HttpPut]
    public IActionResult ChangeTimetable([FromBody] TimetableChange change)
    {
        return Ok(service.UpdateTimetable(change));
    }

    [HttpPost("/trigger")]
    public async Task<IActionResult> TriggerUpdate(CancellationToken ct)
    {
        return Ok(await service.TriggerAsync(ct));
    }

}