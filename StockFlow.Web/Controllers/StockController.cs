using System.Net.Http.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using StockFlow.Web.Models;

namespace StockFlow.Web.Controllers;

[Authorize(Roles = "Admin,Operador")]
public class StockController : Controller
{
    private readonly HttpClient _httpClient;

    public StockController(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("StockFlowApi");
    }

    public async Task<IActionResult> Index()
    {
        var stockItems = await _httpClient.GetFromJsonAsync<List<StockItemViewModel>>("api/stock")
            ?? [];

        return View(stockItems);
    }

    public async Task<IActionResult> Details(int? productVariantId)
    {
        if (productVariantId == null)
        {
            return NotFound();
        }

        var movements = await _httpClient.GetFromJsonAsync<List<StockMovementViewModel>>(
                $"api/stock/movements/{productVariantId}")
            ?? [];

        ViewBag.ProductVariantId = productVariantId.Value;

        return View(movements);
    }

    public async Task<IActionResult> Create(int? productVariantId, int? warehouseId)
    {
        var movement = new StockMovementViewModel
        {
            ProductVariantId = productVariantId ?? 0,
            WarehouseId = warehouseId ?? 0
        };

        FillMovementTypeOptions();
        await FillStockMovementOptionsAsync(movement);

        return View(movement);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("ProductVariantId,WarehouseId,Type,Quantity,Reason")] StockMovementViewModel movement)
    {
        if (ModelState.IsValid)
        {
            var response = await _httpClient.PostAsJsonAsync("api/stock/movements", movement);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Details), new { productVariantId = movement.ProductVariantId });
            }

            ModelState.AddModelError(string.Empty, "Não foi possível registrar a movimentação.");
        }

        FillMovementTypeOptions();
        await FillStockMovementOptionsAsync(movement);

        return View(movement);
    }

    private void FillMovementTypeOptions()
    {
        ViewBag.MovementTypeOptions = new List<SelectListItem>
        {
            new("Entrada", StockMovementTypeViewModel.Entry.ToString()),
            new("Saída", StockMovementTypeViewModel.Exit.ToString()),
            new("Ajuste", StockMovementTypeViewModel.Adjustment.ToString())
        };
    }

    public async Task<IActionResult> Transfer(int? productVariantId, int? fromWarehouseId)
    {
        var transfer = new StockTransferViewModel
        {
            ProductVariantId = productVariantId ?? 0,
            FromWarehouseId = fromWarehouseId ?? 0
        };

        await FillStockTransferOptionsAsync(transfer);

        return View(transfer);
    }

    [HttpPost]
    public async Task<IActionResult> Transfer(StockTransferViewModel transfer)
    {
        if (!ModelState.IsValid)
        {
            await FillStockTransferOptionsAsync(transfer);
            return View(transfer);
        }

        var response = await _httpClient.PostAsJsonAsync("api/stock/transfers", transfer);

        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction(nameof(Details), new { productVariantId = transfer.ProductVariantId });
        }

        ModelState.AddModelError(string.Empty, "Não foi possível registrar a transferência.");
        await FillStockTransferOptionsAsync(transfer);

        return View(transfer);
    }

    private async Task FillStockMovementOptionsAsync(StockMovementViewModel movement)
    {
        movement.VariantOptions = await GetVariantOptionsAsync();
        movement.WarehouseOptions = await GetWarehouseOptionsAsync();
    }

    private async Task FillStockTransferOptionsAsync(StockTransferViewModel transfer)
    {
        transfer.VariantOptions = await GetVariantOptionsAsync();
        transfer.WarehouseOptions = await GetWarehouseOptionsAsync();
    }

    private async Task<List<SelectListItem>> GetVariantOptionsAsync()
    {
        var variants = await _httpClient.GetFromJsonAsync<List<ProductVariantViewModel>>("api/products/variants")
            ?? [];

        return variants.Select(variant => new SelectListItem
        {
            Value = variant.Id.ToString(),
            Text = $"{variant.ProductName} - {variant.Size} - {variant.Color} - {variant.Sku}"
        }).ToList();
    }

    private async Task<List<SelectListItem>> GetWarehouseOptionsAsync()
    {
        var warehouses = await _httpClient.GetFromJsonAsync<List<WarehouseViewModel>>("api/warehouses")
            ?? [];

        return warehouses.Select(warehouse => new SelectListItem
        {
            Value = warehouse.Id.ToString(),
            Text = string.IsNullOrWhiteSpace(warehouse.Location)
                ? warehouse.Name
                : $"{warehouse.Name} - {warehouse.Location}"
        }).ToList();
    }
}
