using Microsoft.AspNetCore.Mvc;
using StockFlow.Web.Models;

namespace StockFlow.Web.Controllers;

public class WarehousesController : Controller
{
    private readonly HttpClient _httpClient;

    public WarehousesController(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("StockFlowApi");
    }

    public async Task<IActionResult> Index()
    {
        var warehouses = await _httpClient.GetFromJsonAsync<List<WarehouseViewModel>>("api/warehouses")
            ?? [];

        return View(warehouses);
    }
}