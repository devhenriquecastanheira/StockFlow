using Microsoft.AspNetCore.Mvc;
using StockFlow.Web.Models;

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
}
