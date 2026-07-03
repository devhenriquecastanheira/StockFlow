using StockFlow.Domain.Entities;

namespace StockFlow.Domain.Interfaces;

public interface ICustomerRepository
{
    Task<CustomerProfile?> GetProfileByUserIdAsync(int userId);
    Task<CustomerAddress?> GetAddressAsync(int userId, int addressId);
    Task AddAddressAsync(CustomerAddress address);
    Task UpdateAddressAsync(CustomerAddress address);
    Task DeleteAddressAsync(CustomerAddress address);
}
