using System.ComponentModel.DataAnnotations;

namespace StockFlow.Web.Models;

public class TagViewModel
{
    public int Id { get; set; }

    [Display(Name = "Nome da Tag")]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Produtos")]
    public List<ProductViewModel> Products { get; set; } = [];
}
