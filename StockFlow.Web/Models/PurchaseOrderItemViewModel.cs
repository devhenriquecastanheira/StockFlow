using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace StockFlow.Web.Models;

public class PurchaseOrderItemViewModel
{
    public int Id { get; set; }
    public int PurchaseOrderId { get; set; }

    [Display(Name = "Produto")]
    public int ProductVariantId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string Size { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;

    [Display(Name = "Quantidade")]
    public int Quantity { get; set; }

    [Display(Name = "Custo Unitário")]
    public decimal UnitCost { get; set; }
    public decimal SubTotal => Quantity * UnitCost;
    public List<SelectListItem> VariantOptions { get; set; } = [];
}
