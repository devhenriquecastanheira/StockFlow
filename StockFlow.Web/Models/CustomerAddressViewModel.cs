using System.ComponentModel.DataAnnotations;

namespace StockFlow.Web.Models;

public class CustomerAddressViewModel
{
    public int Id { get; set; }
    public int CustomerProfileId { get; set; }

    [Display(Name = "Rua")]
    public string Street { get; set; } = string.Empty;

    [Display(Name = "Número")]
    public string Number { get; set; } = string.Empty;

    [Display(Name = "Cidade")]
    public string City { get; set; } = string.Empty;

    [Display(Name = "Estado")]
    public string State { get; set; } = string.Empty;
}
