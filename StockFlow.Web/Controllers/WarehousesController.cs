using Microsoft.AspNetCore.Mvc;
using StockFlow.Web.Models;
using System.Net;

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

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var response = await _httpClient.GetAsync($"api/warehouses/{id}");

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return NotFound();
        }

        response.EnsureSuccessStatusCode();

        var warehouse = await response.Content.ReadFromJsonAsync<WarehouseViewModel>();

        return View(warehouse);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create([Bind("Id,Name,Location")] WarehouseViewModel warehouse)
    {
        if (ModelState.IsValid)
        {
            var response = await _httpClient.PostAsJsonAsync("api/warehouses", warehouse);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, "Não foi possível cadastrar o armazém.");
        }

        return View(warehouse);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var response = await _httpClient.GetAsync($"api/warehouses/{id}");

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return NotFound();
        }

        response.EnsureSuccessStatusCode();

        var warehouse = await response.Content.ReadFromJsonAsync<WarehouseViewModel>();

        return View(warehouse);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Location")] WarehouseViewModel warehouse)
    {
        if (id != warehouse.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/warehouses/{id}", warehouse);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return NotFound();
            }

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, "Não foi possível atualizar o armazém.");
        }

        return View(warehouse);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var response = await _httpClient.GetAsync($"api/warehouses/{id}");

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return NotFound();
        }

        response.EnsureSuccessStatusCode();

        var warehouse = await response.Content.ReadFromJsonAsync<WarehouseViewModel>();

        return View(warehouse);
    }

    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var response = await _httpClient.DeleteAsync($"api/warehouses/{id}");

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return NotFound();
        }

        response.EnsureSuccessStatusCode();

        return RedirectToAction(nameof(Index));
    }
}