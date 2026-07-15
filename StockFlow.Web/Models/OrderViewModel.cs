using System.ComponentModel.DataAnnotations;

namespace StockFlow.Web.Models;

public class OrderViewModel
{
    public int Id { get; set; }
    public int CustomerProfileId { get; set; }

    [Display(Name = "Nome do cliente")]
    public string CustomerName { get; set; } = string.Empty;

    [Display(Name = "Email do cliente")]
    public string CustomerEmail { get; set; } = string.Empty;

    public int? DeliveryAddressId { get; set; }

    [Display(Name = "Rua de entrega")]
    public string DeliveryStreet { get; set; } = string.Empty;

    [Display(Name = "Numero de entrega")]
    public string DeliveryNumber { get; set; } = string.Empty;

    [Display(Name = "Cidade de entrega")]
    public string DeliveryCity { get; set; } = string.Empty;

    [Display(Name = "Estado de entrega")]
    public string DeliveryState { get; set; } = string.Empty;

    [Display(Name = "Status do pedido")]
    public OrderStatusViewModel Status { get; set; }

    [Display(Name = "Data de criação")]
    public DateTime CreatedAt { get; set; }

    [Display(Name = "Itens do pedido")]
    public List<OrderItemViewModel> Items { get; set; } = new List<OrderItemViewModel>();

    public InvoiceViewModel? Invoice { get; set; }

    public decimal Total => Items.Sum(item => item.SubTotal);
}
