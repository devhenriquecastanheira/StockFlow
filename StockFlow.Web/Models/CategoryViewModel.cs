using System.ComponentModel.DataAnnotations;

namespace StockFlow.Web.Models;

public class CategoryViewModel
{
    public int Id { get; set; }

    [Display(Name = "Nome")]
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Descrição")]
    [StringLength(500)]
    public string Description { get; set; } = string.Empty;
}
