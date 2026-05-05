using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AvalWebBackend.Application.Common.Interfaces;
using AvalWebBackend.Application.DTOs;
using AvalWebBackend.Infrastructure.Filters;

namespace AvalWebBackend.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "ADMIN")]

public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IUserService userService, ILogger<UsersController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var data = await _userService.GetAllUsersAsync();
        return Ok(data);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var data = await _userService.GetUserByIdAsync(id);
        return Ok(data);
    }

    [HttpPost]

    [IgnoreCsrf]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest request)
    {
        var data = await _userService.CreateUserAsync(request);
        _logger.LogInformation("User {UserId} created", data.Id);
        return CreatedAtAction(nameof(GetById), new { id = data.Id }, data);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateUserRequest request)
    {
        var data = await _userService.UpdateUserAsync(id, request);
        _logger.LogInformation("User {UserId} updated", id);
        return Ok(data);
    }

    [HttpDelete("{id}")]

    [IgnoreCsrf]
    public async Task<IActionResult> Delete(string id)
    {
        await _userService.DeleteUserAsync(id);
        _logger.LogInformation("User {UserId} deleted", id);
        return NoContent();
    }

    
}