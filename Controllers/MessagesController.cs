using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AvalWebBackend.Application.Common.Interfaces;
using AvalWebBackend.Application.DTOs;
using AvalWebBackend.Infrastructure.Filters;

namespace AvalWebBackend.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "ADMIN,USER")]

public class MessagesController : ControllerBase
{
    private readonly IMessageService _messageService;

    public MessagesController(IMessageService messageService) => _messageService = messageService;

    [HttpGet]
    public async Task<IActionResult> GetByTicket([FromQuery] string ticketId)
    {
        var data = await _messageService.GetMessagesByTicketAsync(ticketId);
        return Ok(data);
    }

    [HttpPost]


    [IgnoreCsrf]


    public async Task<IActionResult> Send([FromBody] SendMessageRequest request)
    {
        var data = await _messageService.SendMessageAsync(request);
        return CreatedAtAction(nameof(GetByTicket), new { ticketId = data.TicketId }, data);
    }

    [HttpPut("{id}/read")]


    [IgnoreCsrf]

    public async Task<IActionResult> MarkAsRead(string id)
    {
        var data = await _messageService.MarkAsReadAsync(id);
        return Ok(data);
    }

    [HttpPut("ticket/{ticketId}/read")]


    [IgnoreCsrf]


    public async Task<IActionResult> MarkTicketMessagesAsRead(string ticketId, [FromQuery] string readerType)
    {
        if (string.IsNullOrEmpty(readerType) || (readerType != "user" && readerType != "admin"))
            return BadRequest(new { message = "نوع خواننده نامعتبر است" });

        await _messageService.MarkMessagesAsReadAsync(ticketId, readerType);
        return Ok(new { message = "پیام‌ها به عنوان خوانده شده علامت‌گذاری شدند" });
    }
}