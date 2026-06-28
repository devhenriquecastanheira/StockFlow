using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace StockFlow.Web.Models;

public class PurchaseOrderViewModel
{
    public int Id { get; set; }

    [Display(Name = "Fornecedor")]
    public int SupplierId { get; set; }
    public string SupplierName { get; set; } = string.Empty;

    [Display(Name = "Data de Criação")]
    public DateTime CreatedAt { get; set; }
    public PurchaseOrderStatusViewModel Status { get; set; }
    public List<PurchaseOrderItemViewModel> Items { get; set; } = [];
    public List<SelectListItem> SupplierOptions { get; set; } = [];
    public List<SelectListItem> WarehouseOptions { get; set; } = [];

    [Display(Name = "Armazém")]
    public int WarehouseId { get; set; }
    public decimal Total => Items.Sum(item => item.SubTotal);
}
