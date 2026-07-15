using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using StockFlow.Web.Models;

namespace StockFlow.Web.Controllers;

[Authorize(Roles = "Admin,Operador,Cliente")]
public class OrdersController : Controller
{
    private readonly HttpClient _httpClient;

    public OrdersController(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("StockFlowApi");
    }

    [Authorize(Roles = "Admin,Operador")]
    public async Task<IActionResult> Index()
    {
        var orders = await _httpClient.GetFromJsonAsync<List<OrderViewModel>>("api/orders")
            ?? new List<OrderViewModel>();

        return View(orders);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var response = await _httpClient.GetAsync($"api/orders/{id}");

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return NotFound();
        }

        response.EnsureSuccessStatusCode();

        var order = await response.Content.ReadFromJsonAsync<OrderViewModel>();

        return View(order);
    }

    [Authorize(Roles = "Admin,Operador")]
    public IActionResult Create()
    {
        return View();
    }

    [Authorize(Roles = "Admin,Operador")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,CustomerName,CustomerEmail,Status,DeliveryAddressId,DeliveryStreet,DeliveryNumber,DeliveryCity,DeliveryState")] OrderViewModel order)
    {
        if (ModelState.IsValid)
        {
            var response = await _httpClient.PostAsJsonAsync("api/orders", order);
            if (response.IsSuccessStatusCode)
            {
                var createdOrder = await response.Content.ReadFromJsonAsync<OrderViewModel>();
                if (createdOrder is not null)
                {
                    return RedirectToAction(nameof(Details), new { id = createdOrder.Id });
                }

                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, "Não foi possível cadastrar o pedido.");
        }

        return View(order);
    }

    [Authorize(Roles = "Admin,Operador")]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var response = await _httpClient.GetAsync($"api/orders/{id}");

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return NotFound();
        }

        response.EnsureSuccessStatusCode();

        var order = await response.Content.ReadFromJsonAsync<OrderViewModel>();

        return View(order);
    }

    [Authorize(Roles = "Admin,Operador")]
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var response = await _httpClient.DeleteAsync($"api/orders/{id}");

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return NotFound();
        }

        response.EnsureSuccessStatusCode();

        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin,Operador")]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var response = await _httpClient.GetAsync($"api/orders/{id}");

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return NotFound();
        }

        response.EnsureSuccessStatusCode();

        var order = await response.Content.ReadFromJsonAsync<OrderViewModel>();

        return View(order);
    }

    [Authorize(Roles = "Admin,Operador")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,CustomerProfileId,CustomerName,CustomerEmail,DeliveryAddressId,DeliveryStreet,DeliveryNumber,DeliveryCity,DeliveryState")] OrderViewModel order)
    {
        if (id != order.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/orders/{id}", order);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return NotFound();
            }

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, "Não foi possível atualizar o pedido.");
        }

        return View(order);
    }

    public async Task<IActionResult> AddItem(int? orderId)
    {
        if (orderId == null)
        {
            return NotFound();
        }

        var response = await _httpClient.GetAsync($"api/orders/{orderId}");

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return NotFound();
        }

        response.EnsureSuccessStatusCode();

        var order = new OrderItemViewModel
        {
            Id = orderId.Value,
            Quantity = 1
        };

        await FillVariantOptionsAsync(order);

        return View(order);
    }

    [HttpPost]
    public async Task<IActionResult> AddItem(int orderId, [Bind("ProductVariantId,Quantity,UnitPrice")] OrderItemViewModel orderItem)
    {
        if (User.IsInRole("Cliente"))
        {
            var salePrice = await GetSalePriceAsync(orderItem.ProductVariantId);
            if (salePrice is null)
            {
                return NotFound();
            }

            orderItem.UnitPrice = salePrice.Value;
        }

        var response = await _httpClient.PostAsJsonAsync($"api/orders/{orderId}/items", orderItem);

        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction(nameof(Details), new { id = orderId });
        }

        orderItem.Id = orderId;
        await FillVariantOptionsAsync(orderItem);

        return View(orderItem);
    }

    private async Task FillVariantOptionsAsync(OrderItemViewModel orderItem)
    {
        var variants = await _httpClient.GetFromJsonAsync<List<ProductVariantViewModel>>("api/products/variants")
            ?? new List<ProductVariantViewModel>();

        orderItem.VariantOptions = variants.Select(v => new SelectListItem
        {
            Value = v.Id.ToString(),
            Text = $"{v.ProductName} - {v.Size} - {v.Color}"
        }).ToList();
    }

    private async Task<decimal?> GetSalePriceAsync(int productVariantId)
    {
        var products = await _httpClient.GetFromJsonAsync<List<ProductViewModel>>("api/products")
            ?? new List<ProductViewModel>();

        foreach (var product in products)
        {
            var variants = await _httpClient.GetFromJsonAsync<List<ProductVariantViewModel>>(
                $"api/products/{product.Id}/variants") ?? new List<ProductVariantViewModel>();

            if (variants.Any(variant => variant.Id == productVariantId))
            {
                return product.SalePrice;
            }
        }

        return null;
    }

    [Authorize(Roles = "Admin,Operador")]
    public async Task<IActionResult> ChangeStatusAsync(int? orderId)
    {
        if (orderId == null)
        {
            return NotFound();
        }

        var response = await _httpClient.GetAsync($"api/orders/{orderId}");

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return NotFound();
        }

        response.EnsureSuccessStatusCode();

        var order = await response.Content.ReadFromJsonAsync<OrderViewModel>();

        return View(order);
    }

    [Authorize(Roles = "Admin,Operador")]
    [HttpPost]
    public async Task<IActionResult> ChangeStatusAsync(int id, [Bind("Status")] OrderViewModel order)
    {
        var newStatus = order.Status;

        var response = await _httpClient.PatchAsJsonAsync($"api/orders/{id}/status", newStatus);

        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction(nameof(Details), new { id = id });
        }
        return View(order);
    }

    public async Task<IActionResult> RemoveItem(int? orderId, int? itemId)
    {
        if (orderId == null || itemId == null)
        {
            return NotFound();
        }

        var response = await _httpClient.GetAsync($"api/orders/{orderId}");

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return NotFound();
        }

        response.EnsureSuccessStatusCode();

        var order = await response.Content.ReadFromJsonAsync<OrderViewModel>();

        if (order is null)
        {
            return NotFound();
        }

        var item = order.Items.FirstOrDefault(i => i.Id == itemId.Value);

        if (item is null)
        {
            return NotFound();
        }

        order.Items = new List<OrderItemViewModel> { item };

        return View(order);
    }

    [HttpPost, ActionName("RemoveItem")]
    public async Task<IActionResult> RemoveItemConfirmed(int orderId, int itemId)
    {
        var response = await _httpClient.DeleteAsync($"api/orders/{orderId}/items/{itemId}");

        return RedirectToAction(nameof(Details), new { id = orderId });
    }

    [HttpPost, ActionName("ConfirmOrder")]
    public async Task<IActionResult> ConfirmOrder(int orderId)
    {
        var response = await _httpClient.PostAsync($"api/orders/{orderId}/confirm", null);

        if (response.IsSuccessStatusCode)
        {
            TempData["Success"] = "Pedido finalizado com sucesso.";
            return RedirectToAction(nameof(Details), new { id = orderId });
        }

        var error = await response.Content.ReadAsStringAsync();

        TempData["Error"] = string.IsNullOrWhiteSpace(error)
            ? "Não foi possível finalizar o pedido."
            : error;

        return RedirectToAction(nameof(Details), new { id = orderId });
    }

    public async Task<IActionResult> Invoice(int orderId)
    {
        var response = await _httpClient.GetAsync($"api/orders/{orderId}/invoice/pdf");

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return NotFound();
        }

        response.EnsureSuccessStatusCode();

        var pdf = await response.Content.ReadAsByteArrayAsync();
        return File(pdf, "application/pdf", $"fatura-{orderId}.pdf");
    }
}
