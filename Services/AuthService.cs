using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CoinViewer.Data;
using CoinViewer.DTOs;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace CoinViewer.Services;

public class AuthService(IConfiguration config, IUserRepository repo) : IAuthService
{
    public (AuthResult, string?) Login(LoginDto login)
    {
        if(string.IsNullOrWhiteSpace(login.Username) || string.IsNullOrWhiteSpace(login.Password))
        {
            return (AuthResult.EMPTY_FIELD, null);
        }
        var user = repo.GetUser(login);
        if (user is null)
        {
            return (AuthResult.DENIED, null);
        }

        var claims = new List<Claim>{
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, login.Username)
        };

        var jwtKey = config["Jwt:Key"];
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(1),
            Issuer = config["Jwt:Issuer"],
            Audience = config["Jwt:Audience"],
            SigningCredentials = creds
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenConfig = tokenHandler.CreateToken(tokenDescriptor);

        return (AuthResult.SUCCESS, tokenHandler.WriteToken(tokenConfig));
    }

    public RegisterResult Register(RegisterDto register)
    {
        if(repo.HasUser(register.Username))
        {
            return RegisterResult.DUBLICATE_USER;
        }

        PasswordStats stats = new(register.Password);
        if(stats.ContainsBadSymbols())
        {
            return RegisterResult.BAD_PASSWORD_SYMBOLS;
        }
        if(!stats.IsAcceptable())
        {
            return RegisterResult.BAD_PASSWORD_FORMAT;
        }

        bool IsAccepted = repo.StoreUser(register);
        if(!IsAccepted)
        {
            return RegisterResult.REGISTER_FAILED_INTERNAL;
        }
        return RegisterResult.SUCCESS;
    }

    class PasswordStats
    {
        const string allowedSymbols = "!@#$%";
        readonly long NumberCount, SmallLetterCount, BigLetterCount, SymbolsCount, BadSymbols, Length;
        public PasswordStats(string password)
        {
            Length = password.Length;
            foreach(char c in password)
            {
                if (char.IsDigit(c))
                {
                    NumberCount++;
                }
                else if (char.IsLower(c))
                {
                    SmallLetterCount++;
                }
                else if (char.IsUpper(c))
                {
                    BigLetterCount++;
                }
                else if (allowedSymbols.Contains(c))
                {
                    SymbolsCount++;
                }
                else
                {
                    BadSymbols++;
                }
            }
        }
        public bool IsAcceptable()
        {
            return 
                BadSymbols == 0 && 
                Length >= 8 && 
                NumberCount > 0 && 
                SmallLetterCount > 0 && 
                BigLetterCount > 0 && 
                SymbolsCount > 0;
        }
        public bool ContainsBadSymbols()
        {
            return BadSymbols > 0;
        }
    }
}