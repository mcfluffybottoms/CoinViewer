using System.Net.Http.Json;
using System.Text.Json;
using CoinViewer.Data;
using CoinViewer.DTOs;
using CoinViewer.Models;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Caching.Distributed;

namespace CoinViewer.Services;

public interface IAPIAccessService
{
    public Task<Coin> GetCoin(CoinSymbol symbol, CancellationToken ct = default);
}