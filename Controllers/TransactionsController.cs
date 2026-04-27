using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AvalWebBackend.Application.Common.Interfaces;
using AvalWebBackend.Application.DTOs;
using AvalWebBackend.Infrastructure.Filters;
using Microsoft.AspNetCore.RateLimiting;

namespace AvalWebBackend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TransactionsController : ControllerBase
{
    private readonly ITransactionService _transactionService;

    public TransactionsController(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    [HttpPost]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> Add([FromBody] CreateTransactionDto dto)
    {
        var result = await _transactionService.AddAsync(dto);
        return Ok(result);
    }

    [HttpPost("batch")]
    [ApiKey]
    [EnableRateLimiting("BatchPolicy")]
    public async Task<IActionResult> AddBatch([FromBody] List<CreateTransactionDto> dtos)
    {
        var results = await _transactionService.AddBatchAsync(dtos);
        return Ok(results);
    }
}