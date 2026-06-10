using System.ComponentModel.DataAnnotations;

namespace StockFlow.Web.Models;

public enum OrderStatusViewModel
{
    [Display(Name = "Pendente")]
    Pending = 1,

    [Display(Name = "Confirmado")]
    Confirmed = 2,

    [Display(Name = "Enviado")]
    Shipped = 3,

    [Display(Name = "Entregue")]
    Delivered = 4
}
