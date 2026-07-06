using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockFlow.Web.Models;

namespace StockFlow.Web.Controllers;

public class AccountController : Controller
{
    private readonly HttpClient _httpClient;

    public AccountController(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("StockFlowApi");
    }

    [AllowAnonymous]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginViewModel login)
    {
        if (!ModelState.IsValid)
        {
            return View(login);
        }

        var response = await _httpClient.PostAsJsonAsync("api/auth/login", login);
        if (!response.IsSuccessStatusCode)
        {
            ModelState.AddModelError(string.Empty, "Email ou senha inválidos.");
            return View(login);
        }

        var auth = await response.Content.ReadFromJsonAsync<AuthResponseViewModel>();
        if (auth is null)
        {
            ModelState.AddModelError(string.Empty, "Não foi possível entrar.");
            return View(login);
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, auth.Name),
            new(ClaimTypes.Role, auth.Role),
            new("Token", auth.Token)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties { ExpiresUtc = auth.ExpiresAt });

        if (auth.Role == UserRoleViewModel.Cliente.ToString())
        {
            return RedirectToAction("Index", "Customers");
        }

        return RedirectToAction("Index", "Home");
    }

    [AllowAnonymous]
    public IActionResult Register()
    {
        return View(new RegisterViewModel());
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegisterViewModel register)
    {
        register.Role = UserRoleViewModel.Cliente;

        if (!ModelState.IsValid)
        {
            return View(register);
        }

        var response = await _httpClient.PostAsJsonAsync("api/auth/register", register);
        if (response.IsSuccessStatusCode)
        {
            TempData["Success"] = "Cadastro criado. Faça login para continuar.";
            return RedirectToAction(nameof(Login));
        }

        ModelState.AddModelError(string.Empty, "Não foi possível criar o cadastro.");
        return View(register);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction(nameof(Login));
    }

    [AllowAnonymous]
    public IActionResult AccessDenied()
    {
        return View();
    }
}
