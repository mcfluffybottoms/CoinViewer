using System.Text;
using CoinViewer.Data;
using CoinViewer.Models;
using CoinViewer.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

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

// AUTH
var jwtSettings = builder.Configuration.GetSection("Jwt");
var jwtKey = jwtSettings["Key"] ?? throw new InvalidOperationException("Missing Jwt:Key");

builder.Services.AddAuthentication(
    options => {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }
).AddJwtBearer(
    options => {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = AuthOptions.ISSUER,
            ValidateAudience = true,
            ValidAudience = AuthOptions.AUDIENCE,
            ValidateLifetime = true,
            IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(jwtKey),
            ValidateIssuerSigningKey = true,
         };
    }
);

var app = builder.Build();

// HTTP pipeline configuration
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();