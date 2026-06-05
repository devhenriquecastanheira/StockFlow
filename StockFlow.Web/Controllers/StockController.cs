using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using StockFlow.Web.Models;

namespace StockFlow.Web.Controllers;

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

    public IActionResult Create(int? productVariantId, int? warehouseId)
    {
        var movement = new StockMovementViewModel
        {
            ProductVariantId = productVariantId ?? 0,
            WarehouseId = warehouseId ?? 0
        };

        FillMovementTypeOptions();

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
}
