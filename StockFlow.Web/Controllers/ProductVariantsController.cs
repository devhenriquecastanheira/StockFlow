using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockFlow.Web.Models;

namespace StockFlow.Web.Controllers;

[Authorize(Roles = "Admin,Operador,Cliente")]
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

        if (product is null)
        {
            return NotFound();
        }

        var variants = await _httpClient.GetFromJsonAsync<List<ProductVariantViewModel>>(
            $"api/products/{productId}/variants");

        product.Variants = variants ?? [];

        return View(product);
    }

    [Authorize(Roles = "Admin,Operador")]
    public async Task<IActionResult> Create(int productId)
    {
        var product = await _httpClient.GetFromJsonAsync<ProductViewModel>(
            $"api/products/{productId}");

        if (product is null)
        {
            return NotFound();
        }

        var variant = new ProductVariantViewModel
        {
            ProductId = product.Id,
            ProductName = product.Name
        };

        return View(variant);
    }

    [Authorize(Roles = "Admin,Operador")]
    [HttpPost]
    public async Task<IActionResult> Create(ProductVariantViewModel variant)
    {
        var response = await _httpClient.PostAsJsonAsync(
            $"api/products/{variant.ProductId}/variants",
            variant);

        if (!response.IsSuccessStatusCode)
        {
            ModelState.AddModelError(string.Empty, "Não foi possível cadastrar a variante.");
            return View(variant);
        }

        return RedirectToAction(nameof(Index), new { productId = variant.ProductId });
    }

    [Authorize(Roles = "Admin,Operador")]
    public async Task<IActionResult> Edit(int id)
    {
        var variant = await _httpClient.GetFromJsonAsync<ProductVariantViewModel>(
            $"api/products/variants/{id}");

        return View(variant);
    }

    [Authorize(Roles = "Admin,Operador")]
    [HttpPost]
    public async Task<IActionResult> Edit(ProductVariantViewModel variant)
    {
        var response = await _httpClient.PutAsJsonAsync(
            $"api/products/variants/{variant.Id}",
            variant);

        if (!response.IsSuccessStatusCode)
        {
            ModelState.AddModelError(string.Empty, "Não foi possível atualizar a variante.");
            return View(variant);
        }

        return RedirectToAction(nameof(Index), new { productId = variant.ProductId });
    }

    [Authorize(Roles = "Admin,Operador")]
    public async Task<IActionResult> Delete(int id)
    {
        var variant = await _httpClient.GetFromJsonAsync<ProductVariantViewModel>(
            $"api/products/variants/{id}");

        return View(variant);
    }

    [Authorize(Roles = "Admin,Operador")]
    [HttpPost]
    public async Task<IActionResult> Delete(ProductVariantViewModel variant)
    {
        var response = await _httpClient.DeleteAsync(
            $"api/products/variants/{variant.Id}");

        if (!response.IsSuccessStatusCode)
        {
            ModelState.AddModelError(string.Empty, "Não foi possível excluir a variante.");
            return View(variant);
        }

        return RedirectToAction(nameof(Index), new { productId = variant.ProductId });
    }
}