using System.ComponentModel.DataAnnotations;

namespace StockFlow.Web.Models;

public class ProductVariantViewModel
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;

    [Display(Name = "Tamanho")]
    public string Size { get; set; } = string.Empty;

    [Display(Name = "Cor")]
    public string Color { get; set; } = string.Empty;

    [Display(Name = "Código de Barras")]
    public string Sku { get; set; } = string.Empty;

    [Display(Name = "Nível Mínimo de Estoque")]
    public int MinimumStockLevel { get; set; }
}
