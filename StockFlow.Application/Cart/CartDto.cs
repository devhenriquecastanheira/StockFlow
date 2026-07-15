namespace StockFlow.Application.Cart;

public class CartDto
{
    public int Id { get; set; }
    public int CustomerProfileId { get; set; }
    public List<CartItemDto> Items { get; set; } = new List<CartItemDto>();
    public decimal Total => Items.Sum(item => item.SubTotal);
}
