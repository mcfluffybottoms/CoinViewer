using CoinViewer.Data;
using CoinViewer.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// builder.Services.AddHttpClient<GeckoAPIAccessService>(client =>
// {
//     client.BaseAddress = new Uri("https://api.coingecko.com/api/v3/");
//     client.DefaultRequestHeaders.Add("x-cg-demo-api-key", "...");
// });
builder.Services.AddScoped<IAPIAccessService, DebugAPIAccessService>();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSingleton<ICoinRepository, InMemoryCoinRepository>();
builder.Services.AddScoped<CryptoDataService>();

builder.Services.AddSingleton<TimetableService>();
builder.Services.AddHostedService<PriceUpdater>();

var app = builder.Build();

// HTTP pipeline configuration
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();