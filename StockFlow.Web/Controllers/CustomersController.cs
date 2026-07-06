using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockFlow.Web.Models;

namespace StockFlow.Web.Controllers;

[Authorize(Roles = "Cliente")]
public class CustomersController : Controller
{
    private readonly HttpClient _httpClient;

    public CustomersController(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("StockFlowApi");
    }

    public async Task<IActionResult> Index()
    {
        var response = await _httpClient.GetAsync("api/customers/me");
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return View(new CustomerProfileViewModel());
        }

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            return RedirectToAction("Login", "Account");
        }

        response.EnsureSuccessStatusCode();

        var profile = await response.Content.ReadFromJsonAsync<CustomerProfileViewModel>();
        return View(profile ?? new CustomerProfileViewModel());
    }

    public IActionResult AddAddress()
    {
        return View(new CustomerAddressViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> AddAddress(CustomerAddressViewModel address)
    {
        if (!ModelState.IsValid)
        {
            return View(address);
        }

        var response = await _httpClient.PostAsJsonAsync("api/customers/me/addresses", address);
        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError(string.Empty, "Não foi possível cadastrar o endereço.");
        return View(address);
    }

    public async Task<IActionResult> EditAddress(int id)
    {
        var profile = await GetProfileAsync();
        var address = profile?.Addresses.FirstOrDefault(a => a.Id == id);

        if (address is null)
        {
            return NotFound();
        }

        return View(address);
    }

    [HttpPost]
    public async Task<IActionResult> EditAddress(int id, CustomerAddressViewModel address)
    {
        if (id != address.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(address);
        }

        var response = await _httpClient.PutAsJsonAsync($"api/customers/me/addresses/{id}", address);
        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError(string.Empty, "Não foi possível atualizar o endereço.");
        return View(address);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteAddress(int id)
    {
        await _httpClient.DeleteAsync($"api/customers/me/addresses/{id}");
        return RedirectToAction(nameof(Index));
    }

    private async Task<CustomerProfileViewModel?> GetProfileAsync()
    {
        var response = await _httpClient.GetAsync("api/customers/me");
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<CustomerProfileViewModel>();
    }
}
