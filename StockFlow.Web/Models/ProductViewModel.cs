using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace StockFlow.Web.Models;

public class ProductViewModel
{
    public int Id { get; set; }

    [Display(Name = "Nome")]
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Descrição")]
    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    [Display(Name = "Preço de custo")]
    [Range(0, double.MaxValue)]
    public decimal CostPrice { get; set; }

    [Display(Name = "Preço de venda")]
    [Range(0, double.MaxValue)]
    public decimal SalePrice { get; set; }

    [Display(Name = "Categoria")]
    [Range(1, int.MaxValue, ErrorMessage = "Selecione uma categoria.")]
    public int CategoryId { get; set; }

    [Display(Name = "Categoria")]
    public string CategoryName { get; set; } = string.Empty;

    public List<SelectListItem> CategoryOptions { get; set; } = [];

    [Display(Name = "Variantes")]
    public List<ProductVariantViewModel> Variants { get; set; } = [];

    [Display(Name = "Imagens")]
    public List<ProductImageViewModel> Images { get; set; } = [];
}
