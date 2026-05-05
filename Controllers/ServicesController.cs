using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AvalWebBackend.Application.Common.Interfaces;
using AvalWebBackend.Application.DTOs;
using AvalWebBackend.Infrastructure.Filters;

namespace AvalWebBackend.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "ADMIN")]
[ValidateCsrfToken]          
public class ServicesController : ControllerBase
{
    private readonly IInvoiceService _invoiceService;

    public ServicesController(IInvoiceService invoiceService)
    {
        _invoiceService = invoiceService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var services = await _invoiceService.GetAllServicesAsync();
        return Ok(services);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateServiceRequest request)
    {
        var service = await _invoiceService.CreateServiceAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = service.Id }, service);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var services = await _invoiceService.GetAllServicesAsync();
        var service = services.FirstOrDefault(s => s.Id == id);
        if (service == null) return NotFound(new { message = "سرویس یافت نشد." });
        return Ok(service);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateServiceRequest request)
    {
        var service = await _invoiceService.UpdateServiceAsync(id, request);
        return Ok(service);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        await _invoiceService.DeleteServiceAsync(id);
        return NoContent();
    }
}