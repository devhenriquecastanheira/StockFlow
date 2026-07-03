namespace StockFlow.Application.Auth;

public interface IAuthService
{
    Task<bool> RegisterAsync(RegisterRequest request);
    Task<AuthResponse?> LoginAsync(LoginRequest request);
}
