using Microsoft.AspNetCore.Mvc;
using StockFlow.Web.Models;

namespace StockFlow.Web.Controllers;

public class ProductVariantsController : Controller
{
    private readonly HttpClient _httpClient;

    public ProductVariantsController(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("StockFlowApi");
    }

    public async Task<IActionResult> Index(int productId)
    {
        var product = await _httpClient.GetFromJsonAsync<ProductViewModel>(
            $"api/products/{productId}");

        var variants = await _httpClient.GetFromJsonAsync<List<ProductVariantViewModel>>(
            $"api/products/{productId}/variants");

        product.Variants = variants ?? [];

        return View(product);
    }

    public async Task<IActionResult> Create(int productId)
    {
        var product = await _httpClient.GetFromJsonAsync<ProductViewModel>(
            $"api/products/{productId}");

        var variant = new ProductVariantViewModel
        {
            ProductId = product.Id,
            ProductName = product.Name
        };

        return View(variant);
    }

    [HttpPost]
    public async Task<IActionResult> Create(ProductVariantViewModel variant)
    {
        var response = await _httpClient.PostAsJsonAsync(
            $"api/products/{variant.ProductId}/variants",
            variant);

        return RedirectToAction(nameof(Index), new { productId = variant.ProductId });
    }

    public async Task<IActionResult> Edit(int id)
    {
        var variant = await _httpClient.GetFromJsonAsync<ProductVariantViewModel>(
            $"api/products/variants/{id}");

        return View(variant);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(ProductVariantViewModel variant)
    {
        var response = await _httpClient.PutAsJsonAsync(
            $"api/products/variants/{variant.Id}",
            variant);

        return RedirectToAction(nameof(Index), new { productId = variant.ProductId });
    }

    public async Task<IActionResult> Delete(int id)
    {
        var variant = await _httpClient.GetFromJsonAsync<ProductVariantViewModel>(
            $"api/products/variants/{id}");

        return View(variant);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(ProductVariantViewModel variant)
    {
        var response = await _httpClient.DeleteAsync(
            $"api/products/variants/{variant.Id}");

        return RedirectToAction(nameof(Index), new { productId = variant.ProductId });
    }
}