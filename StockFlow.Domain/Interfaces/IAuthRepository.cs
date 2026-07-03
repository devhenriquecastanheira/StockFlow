using StockFlow.Domain.Entities;

namespace StockFlow.Domain.Interfaces;

public interface IAuthRepository
{
    Task<User?> AuthenticateAsync(string email, string password);
    Task<User?> GetByEmailAsync(string email);
    Task AddUserAsync(User user);
}
