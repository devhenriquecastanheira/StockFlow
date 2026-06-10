using System.ComponentModel.DataAnnotations;

namespace StockFlow.Web.Models;

public class OrderViewModel
{
    public int Id { get; set; }

    [Display(Name = "Nome do cliente")]
    public string CustomerName { get; set; } = string.Empty;

    [Display(Name = "Email do cliente")]
    public string CustomerEmail { get; set; } = string.Empty;

    [Display(Name = "Status do pedido")]
    public OrderStatusViewModel Status { get; set; }
}
