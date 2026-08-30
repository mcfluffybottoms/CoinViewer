using CoinViewer.DTOs;
using CoinViewer.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoinViewer.Controllers;

[ApiController]
[Route("auth")]
public class AuthController(IAuthService service) : ControllerBase
{
    [HttpPost("login")]
    [ProducesResponseType(typeof(TokenDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status401Unauthorized)]
    public IActionResult Login(LoginDto login)
    {
        var (result, token) = service.Login(login);
        return result switch
        {
            AuthResult.SUCCESS => Ok(new { token }),
            AuthResult.DENIED => Unauthorized(new ErrorDto("Invalid credentials")),
            AuthResult.EMPTY_FIELD => BadRequest(new ErrorDto("Username or password is empty")),
            _ => StatusCode(500),
        };
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(TokenDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status409Conflict)]
    public IActionResult Register(RegisterDto register)
    {
        var result = service.Register(register);
        return result switch
        {
            RegisterResult.SUCCESS => Ok(),
            RegisterResult.DUBLICATE_USER => Conflict("User already exists."),
            RegisterResult.BAD_PASSWORD_SYMBOLS => BadRequest("Password contains invalid symbols."),
            RegisterResult.BAD_PASSWORD_FORMAT => BadRequest("Password format is invalid."),
            RegisterResult.REGISTER_FAILED_INTERNAL => StatusCode(500, "Registration failed."),
            _ => StatusCode(500),
        };
    }
}