using Microsoft.EntityFrameworkCore;
using StockFlow.Domain.Entities;
using StockFlow.Domain.Interfaces;
using StockFlow.Infrastructure.Data;

namespace StockFlow.Infrastructure.Repositories;

public class AuthRepository : IAuthRepository
{
    private readonly StockFlowDbContext _context;

    public AuthRepository(StockFlowDbContext context)
    {
        _context = context;
    }

    public async Task<User?> AuthenticateAsync(string email, string password)
    {
        return await _context.Users
            .Include(user => user.CustomerProfile)
            .FirstOrDefaultAsync(user => user.Email == email && user.Password == password);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .Include(user => user.CustomerProfile)
            .FirstOrDefaultAsync(user => user.Email == email);
    }

    public async Task AddUserAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    }
}
