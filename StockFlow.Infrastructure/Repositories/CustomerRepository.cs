using Microsoft.EntityFrameworkCore;
using StockFlow.Domain.Entities;
using StockFlow.Domain.Interfaces;
using StockFlow.Infrastructure.Data;

namespace StockFlow.Infrastructure.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly StockFlowDbContext _context;

    public CustomerRepository(StockFlowDbContext context)
    {
        _context = context;
    }

    public async Task<CustomerProfile?> GetProfileByUserIdAsync(int userId)
    {
        return await _context.CustomerProfiles
            .Include(profile => profile.User)
            .Include(profile => profile.Addresses)
            .FirstOrDefaultAsync(profile => profile.UserId == userId);
    }

    public async Task<CustomerAddress?> GetAddressAsync(int userId, int addressId)
    {
        return await _context.CustomerAddresses
            .Include(address => address.CustomerProfile)
            .FirstOrDefaultAsync(address =>
                address.Id == addressId &&
                address.CustomerProfile != null &&
                address.CustomerProfile.UserId == userId);
    }

    public async Task AddAddressAsync(CustomerAddress address)
    {
        _context.CustomerAddresses.Add(address);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAddressAsync(CustomerAddress address)
    {
        _context.CustomerAddresses.Update(address);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAddressAsync(CustomerAddress address)
    {
        _context.CustomerAddresses.Remove(address);
        await _context.SaveChangesAsync();
    }
}
