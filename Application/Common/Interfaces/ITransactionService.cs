using AvalWebBackend.Application.DTOs;

namespace AvalWebBackend.Application.Common.Interfaces;

public interface ITransactionService
{
    Task<TransactionDto> AddAsync(CreateTransactionDto dto);
    Task<List<TransactionDto>> AddBatchAsync(IEnumerable<CreateTransactionDto> dtos);
}