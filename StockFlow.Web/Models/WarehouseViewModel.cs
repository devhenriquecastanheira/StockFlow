using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StockFlow.Web.Models;

public class WarehouseViewModel
{
    public int Id { get; set; }

    [Display(Name = "Nome do Armazém")]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Localização")]
    public string Location { get; set; } = string.Empty;
}
