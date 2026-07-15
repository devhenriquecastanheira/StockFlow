using StockFlow.Domain.Entities;
using StockFlow.Domain.Interfaces;

namespace StockFlow.Application.Cart;

public class CartService : ICartService
{
    private readonly ICartRepository _cartRepository;
    private readonly IProductRepository _productRepository;

    public CartService(ICartRepository cartRepository, IProductRepository productRepository)
    {
        _cartRepository = cartRepository;
        _productRepository = productRepository;
    }

    public async Task<CartDto> GetCartAsync(int customerProfileId)
    {
        var cart = await _cartRepository.GetByCustomerProfileIdAsync(customerProfileId);

        if (cart is null)
        {
            cart = new StockFlow.Domain.Entities.Cart
            {
                CustomerProfileId = customerProfileId
            };

            await _cartRepository.AddAsync(cart);
        }

        return new CartDto
        {
            Id = cart.Id,
            CustomerProfileId = cart.CustomerProfileId,
            Items = cart.Items.Select(item => new CartItemDto
            {
                Id = item.Id,
                ProductVariantId = item.ProductVariantId,
                ProductName = item.ProductVariant.Product.Name,
                Size = item.ProductVariant.Size,
                Color = item.ProductVariant.Color,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice
            }).ToList()
        };
    }

    public async Task<CartDto?> AddItemAsync(int customerProfileId, AddCartItemDto item)
    {
        if (item.Quantity <= 0)
        {
            return null;
        }

        var variant = await _productRepository.GetVariantByIdAsync(item.ProductVariantId);
        if (variant is null)
        {
            return null;
        }

        var cart = await _cartRepository.GetByCustomerProfileIdAsync(customerProfileId);

        if (cart is null)
        {
            cart = new StockFlow.Domain.Entities.Cart
            {
                CustomerProfileId = customerProfileId
            };

            await _cartRepository.AddAsync(cart);
        }

        var cartItem = cart.Items.FirstOrDefault(i => i.ProductVariantId == item.ProductVariantId);

        if (cartItem is null)
        {
            cart.Items.Add(new CartItem
            {
                ProductVariantId = item.ProductVariantId,
                ProductVariant = variant,
                Quantity = item.Quantity,
                UnitPrice = variant.Product.SalePrice
            });
        }
        else
        {
            cartItem.Quantity += item.Quantity;
            cartItem.UnitPrice = variant.Product.SalePrice;
        }

        await _cartRepository.UpdateAsync(cart);

        cart = await _cartRepository.GetByCustomerProfileIdAsync(customerProfileId);

        if (cart is null)
        {
            return null;
        }

        return new CartDto
        {
            Id = cart.Id,
            CustomerProfileId = cart.CustomerProfileId,
            Items = cart.Items.Select(item => new CartItemDto
            {
                Id = item.Id,
                ProductVariantId = item.ProductVariantId,
                ProductName = item.ProductVariant.Product.Name,
                Size = item.ProductVariant.Size,
                Color = item.ProductVariant.Color,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice
            }).ToList()
        };
    }

    public async Task<CartDto?> UpdateItemAsync(int customerProfileId, int itemId, UpdateCartItemDto item)
    {
        if (item.Quantity <= 0)
        {
            return null;
        }

        var cart = await _cartRepository.GetByCustomerProfileIdAsync(customerProfileId);
        if (cart is null)
        {
            return null;
        }

        var cartItem = cart.Items.FirstOrDefault(i => i.Id == itemId);
        if (cartItem is null)
        {
            return null;
        }

        cartItem.Quantity = item.Quantity;

        await _cartRepository.UpdateAsync(cart);

        cart = await _cartRepository.GetByCustomerProfileIdAsync(customerProfileId);

        if (cart is null)
        {
            return null;
        }

        return new CartDto
        {
            Id = cart.Id,
            CustomerProfileId = cart.CustomerProfileId,
            Items = cart.Items.Select(item => new CartItemDto
            {
                Id = item.Id,
                ProductVariantId = item.ProductVariantId,
                ProductName = item.ProductVariant.Product.Name,
                Size = item.ProductVariant.Size,
                Color = item.ProductVariant.Color,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice
            }).ToList()
        };
    }

    public async Task<bool> RemoveItemAsync(int customerProfileId, int itemId)
    {
        var cart = await _cartRepository.GetByCustomerProfileIdAsync(customerProfileId);
        if (cart is null)
        {
            return false;
        }

        var item = cart.Items.FirstOrDefault(i => i.Id == itemId);
        if (item is null)
        {
            return false;
        }

        await _cartRepository.DeleteItemAsync(item);
        return true;
    }

    public async Task<bool> ClearCartAsync(int customerProfileId)
    {
        var cart = await _cartRepository.GetByCustomerProfileIdAsync(customerProfileId);
        if (cart is null)
        {
            return false;
        }

        await _cartRepository.DeleteItemsAsync(cart.Items.ToList());
        return true;
    }
}
