using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockFlow.Web.Models;
using System.Net;
using System.Net.Http.Json;

namespace StockFlow.Web.Controllers;

[Authorize(Roles = "Admin,Operador")]
public class TagsController : Controller
{
    private readonly HttpClient _httpClient;

    public TagsController(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("StockFlowApi");
    }

    public async Task<IActionResult> Index()
    {
        var tags = await _httpClient.GetFromJsonAsync<List<TagViewModel>>("api/tags")
            ?? [];

        return View(tags);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var response = await _httpClient.GetAsync($"api/tags/{id}");

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return NotFound();
        }

        response.EnsureSuccessStatusCode();

        var tag = await response.Content.ReadFromJsonAsync<TagViewModel>();

        if (tag is null)
        {
            return NotFound();
        }

        return View(tag);
    }

    public IActionResult Create()
    {
        return View(new TagViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Create([Bind("Id,Name")] TagViewModel tag)
    {
        if (ModelState.IsValid)
        {
            var response = await _httpClient.PostAsJsonAsync("api/tags", tag);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, "Nao foi possivel cadastrar a tag.");
        }

        return View(tag);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var response = await _httpClient.GetAsync($"api/tags/{id}");

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return NotFound();
        }

        response.EnsureSuccessStatusCode();

        var tag = await response.Content.ReadFromJsonAsync<TagViewModel>();

        if (tag is null)
        {
            return NotFound();
        }

        return View(tag);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] TagViewModel tag)
    {
        if (id != tag.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/tags/{id}", tag);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return NotFound();
            }

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, "Nao foi possivel atualizar a tag.");
        }

        return View(tag);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var response = await _httpClient.GetAsync($"api/tags/{id}");

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return NotFound();
        }

        response.EnsureSuccessStatusCode();

        var tag = await response.Content.ReadFromJsonAsync<TagViewModel>();

        if (tag is null)
        {
            return NotFound();
        }

        return View(tag);
    }

    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var response = await _httpClient.DeleteAsync($"api/tags/{id}");

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return NotFound();
        }

        response.EnsureSuccessStatusCode();

        return RedirectToAction(nameof(Index));
    }
}
