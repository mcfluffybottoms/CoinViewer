using CoinViewer.Models;
using CoinViewer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoinViewer.Controllers;

[Authorize]
[ApiController]
[Route("crypto")]
public class CryptoController(CryptoDataService cryptoDataService) : ControllerBase {
    private readonly CryptoDataService _cryptoDataService = cryptoDataService;

    [HttpGet]
    public IActionResult GetCoinList()
    {
        var coinList = _cryptoDataService.GetCoinList();
        return Ok(coinList);
    }

    [HttpPost]
    public async Task<IActionResult> AddCoin([FromBody] CoinSymbol coin)
    {
        var addedCoin = await _cryptoDataService.AddCoinAsync(coin);
        return Ok(addedCoin);
    }

    [HttpGet("/{symbol}")]
    public IActionResult GetCoinInfo(string symbol)
    {
        var coinInfo = _cryptoDataService.GetCoinInfo(new CoinSymbol(symbol));
        return Ok(coinInfo);
    }

    [HttpPut("/{symbol}/refresh")]
    public async Task<IActionResult> RefreshCoin(string symbol)
    {
        var refreshedCoin = await _cryptoDataService.RefreshCoinPriceAsync(new CoinSymbol(symbol));
        return Ok(refreshedCoin);
    }

    [HttpGet("/{symbol}/history")]
    public IActionResult GetCoinHistory(string symbol)
    {
        var coinHistory = _cryptoDataService.GetCoinHistory(new CoinSymbol(symbol));
        return Ok(coinHistory);
    }

    [HttpGet("/{symbol}/stats")]
    public IActionResult GetCoinStats(string symbol)
    {
        var stats = _cryptoDataService.GetCoinStats(new CoinSymbol(symbol));
        return Ok(stats);
    }

    [HttpDelete("/{symbol}")]
    public IActionResult DeleteCoin(string symbol)
    {
        _cryptoDataService.DeleteCoin(new CoinSymbol(symbol));
        return Ok();
    }
}