using StockFlow.Domain.Entities;
using StockFlow.Domain.Interfaces;

namespace StockFlow.Application.Customers;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _repository;

    public CustomerService(ICustomerRepository repository)
    {
        _repository = repository;
    }

    public async Task<CustomerProfile?> GetProfileAsync(int userId)
    {
        return await _repository.GetProfileByUserIdAsync(userId);
    }

    public async Task<CustomerAddress?> AddAddressAsync(int userId, CustomerAddress request)
    {
        var profile = await _repository.GetProfileByUserIdAsync(userId);
        if (profile is null)
        {
            return null;
        }

        var address = new CustomerAddress
        {
            CustomerProfileId = profile.Id,
            Street = request.Street,
            Number = request.Number,
            City = request.City,
            State = request.State
        };

        await _repository.AddAddressAsync(address);

        return address;
    }

    public async Task<bool> UpdateAddressAsync(int userId, int addressId, CustomerAddress request)
    {
        var address = await _repository.GetAddressAsync(userId, addressId);
        if (address is null)
        {
            return false;
        }

        address.Street = request.Street;
        address.Number = request.Number;
        address.City = request.City;
        address.State = request.State;

        await _repository.UpdateAddressAsync(address);

        return true;
    }

    public async Task<bool> DeleteAddressAsync(int userId, int addressId)
    {
        var address = await _repository.GetAddressAsync(userId, addressId);
        if (address is null)
        {
            return false;
        }

        await _repository.DeleteAddressAsync(address);

        return true;
    }
}
