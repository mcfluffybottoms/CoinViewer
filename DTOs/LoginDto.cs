namespace CoinViewer.DTOs;

public class LoginDto
{
    public required string Username { get; set; }
    public required string Password { get; set; }
}

public class RegisterDto
{
    public required string Username { get; set; }
    public required string Password { get; set; }
}

public class TokenDto
{
    public required string Username { get; set; }
    public required string Password { get; set; }
}