using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AvalWebBackend.Application.Common.Interfaces;
using AvalWebBackend.Application.DTOs;

namespace AvalWebBackend.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "ADMIN")]

public class FinancialStatsController : ControllerBase
{
    private readonly IFinancialStatsService _statsService;

    public FinancialStatsController(IFinancialStatsService statsService)
    {
        _statsService = statsService;
    }

    [HttpGet]
    public async Task<ActionResult<FinancialStatsDto>> Get()
    {
        var stats = await _statsService.GetStatsAsync();
        return Ok(stats);
    }
}