using CoinViewer.DTOs;
using CoinViewer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static CoinViewer.Services.CryptoDataService;

namespace CoinViewer.Controllers;

[Authorize]
[ApiController]
[Route("crypto")]
public class CryptoController(CryptoDataService cryptoDataService) : ControllerBase {
    private readonly CryptoDataService _cryptoDataService = cryptoDataService;

    [HttpGet]
    [ProducesResponseType(typeof(CoinsReturnView), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult GetCoinList()
    {
        var coinList = _cryptoDataService.GetCoinList();
        return Ok(new CoinsReturnView([ ..coinList.Select(c => Mapper.ToDto(c))]));
    }

    [HttpPost]
    [ProducesResponseType(typeof(CoinReturnView), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AddCoin([FromBody] CoinSymbolDto coin)
    {
        var (result, addedCoin) = await _cryptoDataService.AddCoinAsync(coin);
        return result switch
        {
            AddResult.ADDED => Ok(new CoinReturnView(addedCoin!)),
            AddResult.CONFLICT => Conflict(new ErrorDto("Symbol is already added.")),
            AddResult.API_NOT_FOUND => BadRequest(new ErrorDto("Symbol does not exist.")),
            _ => StatusCode(500),
        };
    }

    [HttpGet("{symbol}")]
    [ProducesResponseType(typeof(CoinDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status404NotFound)]
    public IActionResult GetCoinInfo(string symbol)
    {
        var coinInfo = _cryptoDataService.GetCoinInfo(new CoinSymbolDto(symbol));
        if(coinInfo is null)
        {
            return NotFound(new ErrorDto("Symbol not found."));
        }
        return Ok(coinInfo);
    }

    [HttpPut("{symbol}/refresh")]
    [ProducesResponseType(typeof(CoinDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RefreshCoin(string symbol)
    {
        var (result, refreshedCoin) = await _cryptoDataService.RefreshCoinPriceAsync(new CoinSymbolDto(symbol));
        return result switch
        {
            RefreshResult.REFRESHED => Ok(refreshedCoin),
            RefreshResult.NOT_FOUND => NotFound(new ErrorDto("Symbol is not added.")),
            RefreshResult.API_NOT_FOUND => NotFound(new ErrorDto("Symbol does not exist when call to API.")),
            _ => StatusCode(500),
        };
    }

    [HttpGet("{symbol}/history")]
    [ProducesResponseType(typeof(CoinHistoryDto), StatusCodes.Status200OK)]
    public IActionResult GetCoinHistory(string symbol)
    {
        var coinHistory = _cryptoDataService.GetCoinHistory(new CoinSymbolDto(symbol));
        return Ok(coinHistory);
    }

    [HttpGet("{symbol}/stats")]
    public IActionResult GetCoinStats(string symbol)
    {
        var stats = _cryptoDataService.GetCoinStats(new CoinSymbolDto(symbol));
        return Ok(stats);
    }

    [HttpDelete("{symbol}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status404NotFound)]
    public IActionResult DeleteCoin(string symbol)
    {
        bool deleted = _cryptoDataService.DeleteCoin(new CoinSymbolDto(symbol));
        if (!deleted)
        {
            return NotFound(new ErrorDto("Symbol was not added."));
        }
        return Ok(new{});
    }
}