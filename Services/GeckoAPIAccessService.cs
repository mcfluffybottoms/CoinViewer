using System.Net.Http.Json;
using System.Text.Json;
using CoinViewer.Data;
using CoinViewer.DTOs;
using CoinViewer.Models;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Caching.Distributed;

namespace CoinViewer.Services;

public class GeckoAPIAccessService(HttpClient client, IDistributedCache cache) : IAPIAccessService
{
    private readonly HttpClient _client = client;
    private readonly IDistributedCache _mapping = cache;
    private readonly TimeSpan expiration = TimeSpan.FromHours(24);

    public class SymbolToId
    {
        public required string Id { get; set; }
        public required string Symbol { get; set; }
        public required string Name { get; set; }
    }
    public class SearchResponse
    {
        public List<SymbolToId> Coins { get; set; } = [];
    }
    public class CostResponse
    {
        public decimal Usd { get; set; }
    }

    private async Task<SymbolToId?> GetCoinMapping(CoinSymbolDto symbol, CancellationToken ct = default)
    {
        string key = $"coin-mapping:{symbol.Symbol.ToLowerInvariant()}";
        byte[]? cachedMapping = await _mapping.GetAsync(key, ct);
        if (cachedMapping is not null)
        {
            return JsonSerializer.Deserialize<SymbolToId>(cachedMapping) ??
                throw new InvalidOperationException($"Cached mapping for '{symbol.Symbol}' is invalid.");
        }
        var coin = await SearchForCoin(symbol, ct);
        if(coin is null)
        {
            return null;
        }
        byte[] json = JsonSerializer.SerializeToUtf8Bytes(coin);

        await _mapping.SetAsync(key, json,
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = expiration }, ct);
        return coin;
    }
    private async Task<decimal> GetCoinPriceById(string coinId, CancellationToken ct = default)
    {
        var url = QueryHelpers.AddQueryString("simple/price", new Dictionary<string, string?>
        {
            ["ids"] = coinId,
            ["vs_currencies"] = "usd",

        });
        var response = await _client.GetFromJsonAsync<Dictionary<string, CostResponse>>(url, ct)
            ?? throw new InvalidOperationException("CoinGecko returned an empty response while looking up price.");
        return response.Values.First().Usd;
    }
    private async Task<SymbolToId?> SearchForCoin(CoinSymbolDto symbol, CancellationToken ct = default)
    {
        string url = QueryHelpers.AddQueryString("search", "query", symbol.Symbol);
        SearchResponse response = await _client.GetFromJsonAsync<SearchResponse>(url, ct) ?? new SearchResponse();
        return response?.Coins.FirstOrDefault(x => x.Symbol.Equals(symbol.Symbol, StringComparison.OrdinalIgnoreCase));
    }
    public async Task<Coin?> GetCoin(CoinSymbolDto symbol, CancellationToken ct = default)
    {
        Console.WriteLine($"BaseAddress: {_client.BaseAddress}");
        var mapping = await GetCoinMapping(symbol, ct);
        if (mapping is null)
        {
            return null;
        }
        decimal price = await GetCoinPriceById(mapping.Id, ct);
        return new Coin
        {
            Symbol = mapping.Symbol,
            Price = price,
            Name = mapping.Name,
            LastUpdated = DateTime.UtcNow
        };
    }
}