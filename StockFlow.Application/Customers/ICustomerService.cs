using StockFlow.Domain.Entities;

namespace StockFlow.Application.Customers;

public interface ICustomerService
{
    Task<CustomerProfile?> GetProfileAsync(int userId);
    Task<CustomerAddress?> AddAddressAsync(int userId, CustomerAddress request);
    Task<bool> UpdateAddressAsync(int userId, int addressId, CustomerAddress request);
    Task<bool> DeleteAddressAsync(int userId, int addressId);
}
