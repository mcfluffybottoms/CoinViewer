using CoinViewer.DTOs;
using CoinViewer.Models;

namespace CoinViewer.Data;

public class SQLiteUserRepository(AppDbContext context) : IUserRepository
{
    public User? GetUser(LoginDto login)
    {
        var user = context.Users.FirstOrDefault(u => u.Username == login.Username);
        if (user is null)
        {
            return null;
        }

        if (!BCrypt.Net.BCrypt.Verify(
            login.Password,
            user.Password
        ))
        {
            return null;
        }

        return user;
    }

    public bool HasUser(string Username)
    {
        return context.Users.Any(u => u.Username == Username);
    }

    public bool StoreUser(RegisterDto register)
    {
        if (HasUser(register.Username))
        {
            return false;
        }

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(register.Password);

        var user = new User
        {
            Username = register.Username,
            Password = passwordHash
        };

        context.Users.Add(user);
        context.SaveChanges();

        return true;
    }
}