using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using StockFlow.Web.Models;

namespace StockFlow.Web.Controllers;

[Authorize(Roles = "Cliente")]
public class CartController : Controller
{
    private readonly HttpClient _httpClient;

    public CartController(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("StockFlowApi");
    }

    public async Task<IActionResult> Index()
    {
        var cart = await _httpClient.GetFromJsonAsync<CartViewModel>("api/cart")
            ?? new CartViewModel();

        return View(cart);
    }

    [HttpPost]
    public async Task<IActionResult> AddItem(AddCartItemViewModel item)
    {
        if (item.Quantity <= 0)
        {
            item.Quantity = 1;
        }

        var response = await _httpClient.PostAsJsonAsync("api/cart/items", item);

        if (!response.IsSuccessStatusCode)
        {
            TempData["Error"] = "Não foi possível adicionar o item ao carrinho.";
            return RedirectToAction("Index", "ProductVariants", new { productId = item.ProductId });
        }

        TempData["Success"] = "Item adicionado ao carrinho.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> UpdateItem(int itemId, int quantity)
    {
        if (quantity <= 0)
        {
            quantity = 1;
        }

        var item = new UpdateCartItemViewModel
        {
            Quantity = quantity
        };

        var response = await _httpClient.PutAsJsonAsync($"api/cart/items/{itemId}", item);

        if (!response.IsSuccessStatusCode)
        {
            TempData["Error"] = "Não foi possível atualizar o item.";
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> RemoveItem(int itemId)
    {
        var response = await _httpClient.DeleteAsync($"api/cart/items/{itemId}");

        if (!response.IsSuccessStatusCode)
        {
            TempData["Error"] = "Não foi possível remover o item.";
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Clear()
    {
        await _httpClient.DeleteAsync("api/cart");
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Checkout()
    {
        var cart = await _httpClient.GetFromJsonAsync<CartViewModel>("api/cart")
            ?? new CartViewModel();

        var profile = await GetCustomerProfileAsync()
            ?? new CustomerProfileViewModel();

        var viewModel = new CheckoutViewModel
        {
            Items = cart.Items,
            Total = cart.Total,
            Addresses = profile.Addresses.ToList()
        };
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Checkout(CheckoutViewModel model)
    {
        var cart = await _httpClient.GetFromJsonAsync<CartViewModel>("api/cart")
            ?? new CartViewModel();

        if (cart.Items.Count == 0)
        {
            ModelState.AddModelError("", "O carrinho está vazio.");
        }

        if (model.SelectedAddressId is null)
        {
            ModelState.AddModelError(nameof(model.SelectedAddressId), "Selecione um endereço de entrega.");
        }

        if (!ModelState.IsValid)
        {
            var profile = await GetCustomerProfileAsync()
                ?? new CustomerProfileViewModel();

            model.Items = cart.Items;
            model.Total = cart.Total;
            model.Addresses = profile.Addresses.ToList();

            return View(model);
        }

        var response = await _httpClient.PostAsJsonAsync("api/orders/checkout", new
        {
            model.SelectedAddressId
        });

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError(
                string.Empty,
                string.IsNullOrWhiteSpace(error)
                    ? "Não foi possível finalizar o checkout."
                    : error);

            var profile = await GetCustomerProfileAsync()
                ?? new CustomerProfileViewModel();

            model.Items = cart.Items;
            model.Total = cart.Total;
            model.Addresses = profile.Addresses.ToList();

            return View(model);
        }

        var order = await response.Content.ReadFromJsonAsync<OrderViewModel>();
        if (order is null)
        {
            TempData["Error"] = "Pedido criado, mas não foi possível abrir os detalhes.";
            return RedirectToAction(nameof(Index));
        }

        return RedirectToAction("Details", "Orders", new { id = order.Id });
    }

    private async Task<CustomerProfileViewModel?> GetCustomerProfileAsync()
    {
        var response = await _httpClient.GetAsync("api/customers/me");

        if (response.StatusCode == HttpStatusCode.NotFound ||
            response.StatusCode == HttpStatusCode.Unauthorized)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<CustomerProfileViewModel>();
    }
}
