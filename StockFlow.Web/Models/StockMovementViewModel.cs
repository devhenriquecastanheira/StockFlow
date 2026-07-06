using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace StockFlow.Web.Models;

public class StockMovementViewModel
{
    public int Id { get; set; }

    [Display(Name = "Variante do produto")]
    [Range(1, int.MaxValue, ErrorMessage = "Informe a variante do produto.")]
    public int ProductVariantId { get; set; }

    [Display(Name = "Armazém")]
    [Range(1, int.MaxValue, ErrorMessage = "Informe o armazém.")]
    public int WarehouseId { get; set; }

    [Display(Name = "Tipo")]
    [EnumDataType(typeof(StockMovementTypeViewModel))]
    public StockMovementTypeViewModel Type { get; set; } = StockMovementTypeViewModel.Entry;

    [Display(Name = "Quantidade")]
    [Range(1, int.MaxValue, ErrorMessage = "A quantidade deve ser maior que zero.")]
    public int Quantity { get; set; }

    [Display(Name = "Data")]
    public DateTime CreatedAt { get; set; }

    [Display(Name = "Motivo")]
    [StringLength(500)]
    public string Reason { get; set; } = string.Empty;

    public List<SelectListItem> VariantOptions { get; set; } = [];
    public List<SelectListItem> WarehouseOptions { get; set; } = [];
}
