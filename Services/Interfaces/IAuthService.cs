using CoinViewer.DTOs;

namespace CoinViewer.Services;

public enum RegisterResult
{
    SUCCESS,
    DUBLICATE_USER,
    BAD_PASSWORD_SYMBOLS,
    BAD_PASSWORD_FORMAT,
    REGISTER_FAILED_INTERNAL
}

public enum AuthResult
{
    SUCCESS,
    EMPTY_FIELD,
    DENIED
}

public interface IAuthService
{
    (AuthResult, string?) Login(LoginDto login);
    RegisterResult Register(RegisterDto register);
}