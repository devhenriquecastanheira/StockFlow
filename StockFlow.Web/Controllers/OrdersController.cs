using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using StockFlow.Web.Models;
using System.Net;

namespace StockFlow.Web.Controllers;

public class OrdersController : Controller
{
    private readonly HttpClient _httpClient;

    public OrdersController(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("StockFlowApi");
    }

    public async Task<IActionResult> Index()
    {
        var orders = await _httpClient.GetFromJsonAsync<List<OrderViewModel>>("api/orders")
            ?? [];

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

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,CustomerName,CustomerEmail,Status")] OrderViewModel order)
    {
        if (ModelState.IsValid)
        {
            var response = await _httpClient.PostAsJsonAsync("api/orders", order);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, "Não foi possível cadastrar o pedido.");
        }

        return View(order);
    }

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

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,CustomerName,CustomerEmail")] OrderViewModel order)
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

        var order = await response.Content.ReadFromJsonAsync<OrderItemViewModel>();

        await FillVariantOptionsAsync(order);

        return View(order);
    }

    [HttpPost]
    public async Task<IActionResult> AddItem(int orderId, [Bind("ProductVariantId,Quantity,UnitPrice")] OrderItemViewModel orderItem)
    {
        var response = await _httpClient.PostAsJsonAsync($"api/orders/{orderId}/items", orderItem);

        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction(nameof(Details), new { id = orderId });
        }

        await FillVariantOptionsAsync(orderItem);

        return View(orderItem);
    }

    private async Task FillVariantOptionsAsync(OrderItemViewModel orderItem)
    {
        var variants = await _httpClient.GetFromJsonAsync<List<ProductVariantViewModel>>("api/products/variants")
            ?? [];

        orderItem.VariantOptions = variants.Select(v => new SelectListItem
        {
            Value = v.Id.ToString(),
            Text = $"{v.ProductName} - {v.Size} - {v.Color}"
        }).ToList();
    }

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

        var item = order?.Items.FirstOrDefault(i => i.Id == itemId.Value);

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
            return RedirectToAction(nameof(Details), new { id = orderId });
        }

        return RedirectToAction(nameof(Details), new { id = orderId });
    }
}
