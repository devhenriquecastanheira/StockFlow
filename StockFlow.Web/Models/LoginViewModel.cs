using System.ComponentModel.DataAnnotations;

namespace StockFlow.Web.Models;

public class LoginViewModel
{
    public string Email { get; set; } = string.Empty;

    [Display(Name = "Senha")]
    public string Password { get; set; } = string.Empty;
}
