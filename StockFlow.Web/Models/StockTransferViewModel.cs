using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace StockFlow.Web.Models;

public class StockTransferViewModel
{
    public int Id { get; set; }

    [Display(Name = "Variante do produto")]
    public int ProductVariantId { get; set; }

    [Display(Name = "Armazém de origem")]
    public int FromWarehouseId { get; set; }

    [Display(Name = "Armazém de destino")]
    public int ToWarehouseId { get; set; }

    [Display(Name = "Quantidade")]
    public int Quantity { get; set; }
    public DateTime CreatedAt { get; set; }

    public List<SelectListItem> VariantOptions { get; set; } = [];
    public List<SelectListItem> WarehouseOptions { get; set; } = [];
}
