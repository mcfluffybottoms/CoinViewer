using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace CoinViewer.Models;
public class AuthOptions
{
    public const string ISSUER = "CoinViewerAuthServer";
    public const string AUDIENCE = "CoinViewerUser";
    public static SymmetricSecurityKey GetSymmetricSecurityKey(string key) => new(Encoding.UTF8.GetBytes(key));
}