using Microsoft.AspNetCore.Mvc;
using StockFlow.Application.PurchaseOrders;
using StockFlow.Domain.Entities;

namespace StockFlow.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PurchaseOrdersController : ControllerBase
{
    private readonly IPurchaseOrderService _purchaseOrderService;

    public PurchaseOrdersController(IPurchaseOrderService purchaseOrderService)
    {
        _purchaseOrderService = purchaseOrderService;
    }

    [HttpGet]
    public async Task<ActionResult<List<PurchaseOrder>>> GetAll()
    {
        return Ok(await _purchaseOrderService.GetPurchaseOrdersAsync());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PurchaseOrder>> GetById(int id)
    {
        var purchaseOrder = await _purchaseOrderService.GetPurchaseOrderAsync(id);

        if (purchaseOrder is null)
        {
            return NotFound();
        }

        return Ok(purchaseOrder);
    }

    [HttpPost]
    public async Task<ActionResult<PurchaseOrder>> Create(PurchaseOrder purchaseOrder)
    {
        var created = await _purchaseOrderService.AddPurchaseOrderAsync(purchaseOrder);

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPost("{purchaseOrderId}/items")]
    public async Task<ActionResult<PurchaseOrder>> AddItem(int purchaseOrderId, PurchaseOrderItem item)
    {
        try
        {
            var purchaseOrder = await _purchaseOrderService
                .AddPurchaseOrderItemAsync(purchaseOrderId, item);

            if (purchaseOrder is null)
            {
                return NotFound();
            }

            return Ok(purchaseOrder);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{id}/receive")]
    public async Task<ActionResult<PurchaseOrder>> Receive(int id, [FromQuery] int warehouseId)
    {
        try
        {
            var purchaseOrder = await _purchaseOrderService.ReceiveAsync(id, warehouseId);

            if (purchaseOrder is null)
            {
                return NotFound();
            }

            return Ok(purchaseOrder);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}