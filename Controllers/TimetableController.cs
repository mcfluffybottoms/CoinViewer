using CoinViewer.DTOs;
using CoinViewer.Models;
using CoinViewer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoinViewer.Controllers;

[Authorize]
[ApiController]
[Route("schedule")]
public class TimetableController(TimetableService service) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(CoinDto), StatusCodes.Status200OK)]
    public IActionResult GetTimetable()
    {
        Timetable table = service.GetTimetable();
        return Ok(Mapper.ToDto(table));
    }

    [HttpPut]
    [ProducesResponseType(typeof(TimetableChangeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status500InternalServerError)]
    public IActionResult ChangeTimetable([FromBody] TimetableChangeDto change)
    {
        try
        {
            var changeResult = service.UpdateTimetable(change);
            return changeResult is null ? 
                BadRequest(new ErrorDto("Timer should be in range [10, 3600] seconds.")) : 
                Ok(service.UpdateTimetable(change));
        } catch (Exception) {
            return StatusCode(500, new ErrorDto("Internal server error while changing timetable"));
        }
    }

    [HttpPost("trigger")]
    [ProducesResponseType(typeof(ReloadResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> TriggerUpdate(CancellationToken ct)
    {
        try
        {
            return Ok(await service.TriggerAsync(ct));
        } catch (Exception) {
            return StatusCode(500, new ErrorDto("Internal server error while force-updating"));
        }
        
    }

}