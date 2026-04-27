using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AvalWebBackend.Application.Common.Interfaces;
using AvalWebBackend.Application.DTOs;

namespace AvalWebBackend.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "ADMIN,USER")]
public class TicketsController : ControllerBase
{
    private readonly ITicketService _ticketService;

    public TicketsController(ITicketService ticketService)
    {
        _ticketService = ticketService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? userId)
    {
        var data = await _ticketService.GetAllTicketsAsync(userId);
        return Ok(data);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var data = await _ticketService.GetTicketByIdAsync(id);
        return Ok(data);
    }

    [HttpPost]
    [RequestSizeLimit(10 * 1024 * 1024)]
    public async Task<IActionResult> Create([FromBody] CreateTicketRequest request)
    {
        var data = await _ticketService.CreateTicketAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = data.Id }, data);
    }

    [HttpPut("{id}")]
    [RequestSizeLimit(10 * 1024 * 1024)]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateTicketRequest request)
    {
        var data = await _ticketService.UpdateTicketAsync(id, request);
        return Ok(data);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        await _ticketService.DeleteTicketAsync(id);
        return NoContent();
    }

    [HttpGet("{id}/file")]
    public async Task<IActionResult> GetFile(string id)
    {
        var (fileBytes, contentType, fileName) = await _ticketService.GetTicketFileAsync(id);
        return File(fileBytes, contentType, fileName);
    }
}