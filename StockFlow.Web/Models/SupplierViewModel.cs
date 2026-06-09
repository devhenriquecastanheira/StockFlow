using System.ComponentModel.DataAnnotations;

namespace StockFlow.Web.Models;

public class SupplierViewModel
{
    public int Id { get; set; }

    [Display(Name = "Nome")]
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "E-mail")]
    [Required]
    [EmailAddress]
    [StringLength(150)]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "Telefone")]
    [Required]
    [StringLength(30)]
    public string Phone { get; set; } = string.Empty;
}
