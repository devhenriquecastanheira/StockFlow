using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using StockFlow.Web.Models;

namespace StockFlow.Web.Controllers;

public class ProductsController : Controller
{
    private readonly HttpClient _httpClient;

    public ProductsController(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("StockFlowApi");
    }

    public async Task<IActionResult> Index()
    {
        var products = await _httpClient.GetFromJsonAsync<List<ProductViewModel>>("api/products")
            ?? [];
        var categories = await GetCategoriesAsync();

        foreach (var product in products)
        {
            product.CategoryName = categories.FirstOrDefault(category => category.Id == product.CategoryId)?.Name ?? "-";
        }

        return View(products);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var response = await _httpClient.GetAsync($"api/products/{id}");

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return NotFound();
        }

        response.EnsureSuccessStatusCode();

        var product = await response.Content.ReadFromJsonAsync<ProductViewModel>();
        await FillCategoryNameAsync(product);

        return View(product);
    }

    public async Task<IActionResult> Create()
    {
        var product = new ProductViewModel();
        await FillCategoryOptionsAsync(product);

        return View(product);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Name,Description,CostPrice,SalePrice,CategoryId")] ProductViewModel product)
    {
        if (ModelState.IsValid)
        {
            var response = await _httpClient.PostAsJsonAsync("api/products", product);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            await AddApiErrorAsync(response, "Não foi possível cadastrar o produto.");
        }

        await FillCategoryOptionsAsync(product);

        return View(product);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var response = await _httpClient.GetAsync($"api/products/{id}");

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return NotFound();
        }

        response.EnsureSuccessStatusCode();

        var product = await response.Content.ReadFromJsonAsync<ProductViewModel>();
        await FillCategoryOptionsAsync(product);

        return View(product);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,CostPrice,SalePrice,CategoryId")] ProductViewModel product)
    {
        if (id != product.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/products/{id}", product);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return NotFound();
            }

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            await AddApiErrorAsync(response, "Não foi possível atualizar o produto.");
        }

        await FillCategoryOptionsAsync(product);

        return View(product);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var response = await _httpClient.GetAsync($"api/products/{id}");

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return NotFound();
        }

        response.EnsureSuccessStatusCode();

        var product = await response.Content.ReadFromJsonAsync<ProductViewModel>();
        await FillCategoryNameAsync(product);

        return View(product);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var response = await _httpClient.DeleteAsync($"api/products/{id}");

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return NotFound();
        }

        response.EnsureSuccessStatusCode();

        return RedirectToAction(nameof(Index));
    }

    private async Task FillCategoryOptionsAsync(ProductViewModel? product)
    {
        if (product is null)
        {
            return;
        }

        var categories = await GetCategoriesAsync();
        product.CategoryOptions = categories
            .Select(category => new SelectListItem(category.Name, category.Id.ToString()))
            .ToList();
    }

    private async Task FillCategoryNameAsync(ProductViewModel? product)
    {
        if (product is null)
        {
            return;
        }

        var categories = await GetCategoriesAsync();
        product.CategoryName = categories.FirstOrDefault(category => category.Id == product.CategoryId)?.Name ?? "-";
    }

    private async Task<List<CategoryViewModel>> GetCategoriesAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<CategoryViewModel>>("api/categories")
            ?? [];
    }

    private async Task AddApiErrorAsync(HttpResponseMessage response, string fallbackMessage)
    {
        var responseBody = await response.Content.ReadAsStringAsync();

        if (string.IsNullOrWhiteSpace(responseBody))
        {
            ModelState.AddModelError(string.Empty, fallbackMessage);
            return;
        }

        ModelState.AddModelError(string.Empty, $"{fallbackMessage} {responseBody}");
    }
}
