using AvalWebBackend.Application.DTOs;

namespace AvalWebBackend.Application.Common.Interfaces;

public interface ITicketService
{
    Task<IEnumerable<TicketDto>> GetAllTicketsAsync(string? userId);
    Task<TicketDto> GetTicketByIdAsync(string id);
    Task<TicketDto> CreateTicketAsync(CreateTicketRequest request);
    Task<TicketDto> UpdateTicketAsync(string id, UpdateTicketRequest request);
    Task DeleteTicketAsync(string id);
    Task<(byte[] fileBytes, string contentType, string fileName)> GetTicketFileAsync(string id);
}