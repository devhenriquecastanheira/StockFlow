using StockFlow.Domain.Entities;

namespace StockFlow.Domain.Interfaces;

public interface ICartRepository
{
    Task<Cart?> GetByCustomerProfileIdAsync(int customerProfileId);
    Task AddAsync(Cart cart);
    Task UpdateAsync(Cart cart);
    Task DeleteItemAsync(CartItem item);
    Task DeleteItemsAsync(List<CartItem> items);
}
