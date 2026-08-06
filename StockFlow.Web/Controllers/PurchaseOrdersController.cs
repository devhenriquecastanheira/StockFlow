using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using StockFlow.Web.Models;

namespace StockFlow.Web.Controllers;

[Authorize(Roles = "Admin,Operador")]
public class PurchaseOrdersController : Controller
{
    private readonly HttpClient _httpClient;

    public PurchaseOrdersController(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("StockFlowApi");
    }

    public async Task<IActionResult> Index()
    {
        var orders = await _httpClient.GetFromJsonAsync<List<PurchaseOrderViewModel>>("api/purchaseorders") ?? [];

        var suppliers = await _httpClient.GetFromJsonAsync<List<SupplierViewModel>>("api/suppliers") ?? [];

        foreach (var order in orders)
        {
            order.SupplierName = suppliers.FirstOrDefault(s => s.Id == order.SupplierId)?.Name ?? "";
        }

        return View(orders);
    }

    public async Task<IActionResult> Create()
    {
        var order = new PurchaseOrderViewModel();
        await FillSupplierOptionsAsync(order);

        return View(order);
    }

    [HttpPost]
    public async Task<IActionResult> Create(PurchaseOrderViewModel order)
    {
        var response = await _httpClient.PostAsJsonAsync("api/purchaseorders", order);

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(int id)
    {
        var order = await _httpClient.GetFromJsonAsync<PurchaseOrderViewModel>($"api/purchaseorders/{id}");

        if (order is null)
        {
            return NotFound();
        }

        var suppliers = await _httpClient.GetFromJsonAsync<List<SupplierViewModel>>("api/suppliers") ?? [];

        order.SupplierName = suppliers.FirstOrDefault(s => s.Id == order.SupplierId)?.Name ?? "";

        var variants = await _httpClient.GetFromJsonAsync<List<ProductVariantViewModel>>("api/products/variants") ?? [];

        foreach (var item in order.Items)
        {
            var variant = variants.FirstOrDefault(v => v.Id == item.ProductVariantId);
            item.ProductName = variant?.ProductName ?? "";
            item.Size = variant?.Size ?? "";
            item.Color = variant?.Color ?? "";
        }

        await FillWarehouseOptionsAsync(order);

        return View(order);
    }

    public async Task<IActionResult> AddItem(int purchaseOrderId)
    {
        var item = new PurchaseOrderItemViewModel
        {
            PurchaseOrderId = purchaseOrderId
        };

        await FillVariantOptionsAsync(item);

        return View(item);
    }

    [HttpPost]
    public async Task<IActionResult> AddItem(PurchaseOrderItemViewModel item)
    {
        var response = await _httpClient.PostAsJsonAsync($"api/purchaseorders/{item.PurchaseOrderId}/items", item);

        return RedirectToAction(nameof(Details), new { id = item.PurchaseOrderId });
    }

    [HttpPost]
    public async Task<IActionResult> Receive(int id, int warehouseId)
    {
        var response = await _httpClient.PostAsync($"api/purchaseorders/{id}/receive?warehouseId={warehouseId}", null);

        return RedirectToAction(nameof(Details), new { id });
    }

    private async Task FillSupplierOptionsAsync(PurchaseOrderViewModel order)
    {
        var suppliers = await _httpClient.GetFromJsonAsync<List<SupplierViewModel>>("api/suppliers") ?? [];

        order.SupplierOptions = suppliers.Select(supplier => new SelectListItem
        {
            Value = supplier.Id.ToString(),
            Text = supplier.Name
        }).ToList();
    }

    private async Task FillWarehouseOptionsAsync(PurchaseOrderViewModel order)
    {
        var warehouses = await _httpClient.GetFromJsonAsync<List<WarehouseViewModel>>("api/warehouses")?? [];

        order.WarehouseOptions = warehouses.Select(warehouse => new SelectListItem
        {
            Value = warehouse.Id.ToString(),
            Text = warehouse.Name
        }).ToList();
    }

    private async Task FillVariantOptionsAsync(PurchaseOrderItemViewModel item)
    {
        var variants = await _httpClient.GetFromJsonAsync<List<ProductVariantViewModel>>("api/products/variants")?? [];

        item.VariantOptions = variants.Select(variant => new SelectListItem
        {
            Value = variant.Id.ToString(),
            Text = $"{variant.ProductName} - {variant.Size} - {variant.Color}"
        }).ToList();
    }
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var response = await _httpClient.GetAsync($"api/purchaseorders/{id}");

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return NotFound();
        }

        response.EnsureSuccessStatusCode();

        var order = await response.Content.ReadFromJsonAsync<PurchaseOrderViewModel>();

        if (order is null)
        {
            return NotFound();
        }

        var suppliers = await _httpClient.GetFromJsonAsync<List<SupplierViewModel>>("api/suppliers") ?? [];
        order.SupplierName = suppliers.FirstOrDefault(s => s.Id == order.SupplierId)?.Name ?? "";

        return View(order);
    }

    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var response = await _httpClient.DeleteAsync($"api/purchaseorders/{id}");

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return NotFound();
        }

        response.EnsureSuccessStatusCode();

        return RedirectToAction(nameof(Index));
    }
}
