using CoinViewer.DTOs;

namespace CoinViewer.Models;

public class User
{
    public required long Id { get; set; }
    public required string Username { get; set; }
    public required string Password { get; set; }
}