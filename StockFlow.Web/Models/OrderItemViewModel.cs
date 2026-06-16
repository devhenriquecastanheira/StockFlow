using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace StockFlow.Web.Models;

public class OrderItemViewModel
{
    public int Id { get; set; }

    [Display(Name = "ID da variante do produto")]
    public int ProductVariantId { get; set; }

    [Display(Name = "Nome do produto")]
    public string ProductName { get; set; } = string.Empty;

    [Display(Name = "Tamanho")]
    public string Size { get; set; } = string.Empty;

    [Display(Name = "Cor")]
    public string Color { get; set; } = string.Empty;

    [Display(Name = "Quantidade")]
    public int Quantity { get; set; }

    [Display(Name = "Preço unitário")]
    public decimal UnitPrice { get; set; }

    public decimal SubTotal => Quantity * UnitPrice;

    public List<SelectListItem> VariantOptions { get; set; } = [];
}
