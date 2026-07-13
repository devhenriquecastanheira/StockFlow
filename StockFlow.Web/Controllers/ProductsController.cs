using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using StockFlow.Web.Models;

namespace StockFlow.Web.Controllers;

[Authorize(Roles = "Admin,Operador,Cliente")]
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
            product.Images ??= [];
        }

        ViewBag.ApiBaseUrl = _httpClient.BaseAddress?.ToString().TrimEnd('/');

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
        if (product is null)
        {
            return NotFound();
        }

        product.Images ??= [];
        await FillCategoryNameAsync(product);
        ViewBag.ApiBaseUrl = _httpClient.BaseAddress?.ToString().TrimEnd('/');

        return View(product);
    }

    [Authorize(Roles = "Admin,Operador")]
    public async Task<IActionResult> Create()
    {
        var product = new ProductViewModel();
        await FillCategoryOptionsAsync(product);

        return View(product);
    }

    [Authorize(Roles = "Admin,Operador")]
    [HttpPost]
    public async Task<IActionResult> Create([Bind("Id,Name,Description,CostPrice,SalePrice,CategoryId")] ProductViewModel product)
    {
        if (ModelState.IsValid)
        {
            var response = await _httpClient.PostAsJsonAsync("api/products", product);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, "Não foi possível cadastrar o produto.");
        }

        await FillCategoryOptionsAsync(product);

        return View(product);
    }

    [Authorize(Roles = "Admin,Operador")]
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
        if (product is null)
        {
            return NotFound();
        }

        product.Images ??= [];
        await FillCategoryOptionsAsync(product);
        ViewBag.ApiBaseUrl = _httpClient.BaseAddress?.ToString().TrimEnd('/');

        return View(product);
    }

    [Authorize(Roles = "Admin,Operador")]
    [HttpPost]
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

            ModelState.AddModelError(string.Empty, "Não foi possível atualizar o produto.");
        }

        await FillCategoryOptionsAsync(product);

        return View(product);
    }

    [Authorize(Roles = "Admin,Operador")]
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
        if (product is null)
        {
            return NotFound();
        }

        product.Images ??= [];
        await FillCategoryNameAsync(product);

        return View(product);
    }

    [Authorize(Roles = "Admin,Operador")]
    [HttpPost, ActionName("Delete")]
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

    [Authorize(Roles = "Admin,Operador")]
    [HttpPost]
    public async Task<IActionResult> UploadImage(int productId, IFormFile imageFile, bool isMain = false)
    {
        if (imageFile is null || imageFile.Length == 0)
        {
            return RedirectToAction(nameof(Details), new { id = productId });
        }

        using var content = new MultipartFormDataContent();
        using var fileContent = new StreamContent(imageFile.OpenReadStream());

        content.Add(fileContent, "imageFile", imageFile.FileName);
        content.Add(new StringContent(isMain.ToString().ToLowerInvariant()), "isMain");

        var response = await _httpClient.PostAsync($"api/products/{productId}/images", content);

        if (!response.IsSuccessStatusCode)
        {
            TempData["Error"] = await response.Content.ReadAsStringAsync();
        }

        return RedirectToAction(nameof(Details), new { id = productId });
    }

    [Authorize(Roles = "Admin,Operador")]
    [HttpPost]
    public async Task<IActionResult> SetMainImage(int productId, int imageId)
    {
        await _httpClient.PutAsync($"api/products/{productId}/images/{imageId}/main", null);

        return RedirectToAction(nameof(Details), new { id = productId });
    }

    [Authorize(Roles = "Admin,Operador")]
    [HttpPost]
    public async Task<IActionResult> DeleteImage(int productId, int imageId)
    {
        await _httpClient.DeleteAsync($"api/products/{productId}/images/{imageId}");

        return RedirectToAction(nameof(Details), new { id = productId });
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

}
