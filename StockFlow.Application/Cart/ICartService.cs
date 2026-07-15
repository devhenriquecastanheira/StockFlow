namespace StockFlow.Application.Cart;

public interface ICartService
{
    Task<CartDto> GetCartAsync(int customerProfileId);
    Task<CartDto?> AddItemAsync(int customerProfileId, AddCartItemDto item);
    Task<CartDto?> UpdateItemAsync(int customerProfileId, int itemId, UpdateCartItemDto item);
    Task<bool> RemoveItemAsync(int customerProfileId, int itemId);
    Task<bool> ClearCartAsync(int customerProfileId);
}
