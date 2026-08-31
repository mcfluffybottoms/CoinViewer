using System.Text;
using CoinViewer.Data;
using CoinViewer.Models;
using CoinViewer.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;


static void AddAuthServices(WebApplicationBuilder builder)
{
    builder.Services.AddScoped<IAuthService, AuthService>();
    var repositorySettings = builder.Configuration.GetSection("Repository");
    if (repositorySettings["AuthType"] == "InMemory")
    {
        builder.Services.AddSingleton<IUserRepository, InMemoryUserRepository>();
    }
    else if (repositorySettings["AuthType"] == "SQLite")
    {

    }
    else
    {
        throw new InvalidOperationException($"Unsupported repository type: '{repositorySettings["Type"]}'");
    }

    var jwtSettings = builder.Configuration.GetSection("Jwt");
    var jwtKey = jwtSettings["Key"] ?? throw new InvalidOperationException("Missing Jwt:Key");
    var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey!));
    var validIssuer = jwtSettings["Issuer"];
    var validAudience = jwtSettings["Audience"];

    builder.Services.AddAuthentication(
        options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }
    ).AddJwtBearer(
        options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = validIssuer,
                ValidateAudience = true,
                ValidAudience = validAudience,
                ValidateLifetime = true,
                IssuerSigningKey = symmetricSecurityKey,
                ValidateIssuerSigningKey = true,
            };
        }
    );
}

static void AddCryptoSearchServices(WebApplicationBuilder builder)
{
    builder.Services.AddScoped<CryptoDataService>();
    var repositorySettings = builder.Configuration.GetSection("Repository");
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlite(
            builder.Configuration.GetConnectionString("DefaultConnection")
        )
    );
    if (repositorySettings["CryptoType"] == "InMemory")
    {
        builder.Services.AddSingleton<ICoinRepository, InMemoryCoinRepository>();
    }
    else if (repositorySettings["CryptoType"] == "SQLite")
    {
        builder.Services.AddScoped<ICoinRepository, SQLiteCoinRepository>();
    }
    else
    {
        throw new InvalidOperationException(
            $"Unsupported repository type: '{repositorySettings["Type"]}'");
    }

}

static void AddAPIConnectionServices(WebApplicationBuilder builder)
{
    var APISettings = builder.Configuration.GetSection("API");
    if (APISettings["Type"] == "GeckoAPI")
    {
        builder.Services.AddHttpClient<GeckoAPIAccessService>(client =>
        {
            client.BaseAddress = new Uri("https://api.coingecko.com/api/v3/");
            client.DefaultRequestHeaders.Add("x-cg-demo-api-key", "...");
        });
    }
    else if (APISettings["Type"] == "Debug")
    {
        builder.Services.AddScoped<IAPIAccessService, DebugAPIAccessService>();
    }
    else
    {
        throw new InvalidOperationException(
            $"Unsupported API type: '{APISettings["Type"]}'");
    }

}

static void AddTimetableServices(WebApplicationBuilder builder)
{
    builder.Services.AddSingleton<TimetableService>();
    builder.Services.AddHostedService<PriceUpdater>();
}


var builder = WebApplication.CreateBuilder(args);

// ---------- SETUP ---------- //
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Enter your JWT token"
        }
    );

    options.AddSecurityRequirement(document =>
    new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
    });
});

// Add cache
builder.Services.AddDistributedMemoryCache();

// Add services
AddAuthServices(builder);
AddAPIConnectionServices(builder);
AddCryptoSearchServices(builder);
AddTimetableServices(builder);

// ---------- BUILD ---------- //
var app = builder.Build();

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