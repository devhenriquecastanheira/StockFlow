using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockFlow.Application.Tags;
using StockFlow.Domain.Entities;

namespace StockFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Operador")]
public class TagsController : ControllerBase
{
    private readonly ITagService _tagService;

    public TagsController(ITagService tagService)
    {
        _tagService = tagService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<List<Tag>>> GetAll()
    {
        var tags = await _tagService.GetAllAsync();

        return Ok(tags);
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<Tag>> GetById(int id)
    {
        var tag = await _tagService.GetByIdAsync(id);

        if (tag is null)
        {
            return NotFound();
        }

        return Ok(tag);
    }

    [HttpPost]
    public async Task<ActionResult<Tag>> Create(Tag tag)
    {
        var createdTag = await _tagService.CreateAsync(tag);

        return CreatedAtAction(
            nameof(GetById),
            new { id = createdTag.Id },
            createdTag);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<Tag>> Update(int id, Tag tag)
    {
        var updatedTag = await _tagService.UpdateAsync(id, tag);

        if (updatedTag is null)
        {
            return NotFound();
        }

        return Ok(updatedTag);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _tagService.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
