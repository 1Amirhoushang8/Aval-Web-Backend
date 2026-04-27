using Microsoft.AspNetCore.Mvc;
using AvalWebBackend.Infrastructure.Persistence;

namespace AvalWebBackend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class HealthController : ControllerBase
{
    private readonly JsonDataService _dataService;
    private readonly ILogger<HealthController> _logger;

    public HealthController(JsonDataService dataService, ILogger<HealthController> logger)
    {
        _dataService = dataService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        try
        {
            var db = await _dataService.ReadAsync();

            return Ok(new
            {
                status = "healthy",
                users = db.Users?.Count ?? 0,
                tickets = db.Tickets?.Count ?? 0,
                transactions = db.Transactions?.Count ?? 0,
                messages = db.Messages?.Count ?? 0
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Health check failed");
            return StatusCode(503, new { status = "unhealthy" });
        }
    }
}