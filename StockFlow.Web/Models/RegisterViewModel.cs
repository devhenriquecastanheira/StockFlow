using System.ComponentModel.DataAnnotations;

namespace StockFlow.Web.Models;

public class RegisterViewModel
{
    [Display(Name = "Nome")]
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    [Display(Name = "Senha")]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Confirmar Senha")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Display(Name = "Função")]
    public UserRoleViewModel Role { get; set; } = UserRoleViewModel.Cliente;
}
