using CoinViewer.DTOs;
using CoinViewer.Models;

namespace CoinViewer.Data;

public interface IUserRepository
{
    public User? GetUser(LoginDto user);
    public bool StoreUser(RegisterDto user);
    public bool HasUser(string Username);
} 