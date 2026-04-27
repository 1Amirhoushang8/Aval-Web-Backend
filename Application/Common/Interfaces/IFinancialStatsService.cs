using AvalWebBackend.Application.DTOs;

namespace AvalWebBackend.Application.Common.Interfaces;

public interface IFinancialStatsService
{
    Task<FinancialStatsDto> GetStatsAsync();
}