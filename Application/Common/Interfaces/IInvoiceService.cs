using AvalWebBackend.Application.DTOs;

namespace AvalWebBackend.Application.Common.Interfaces;

public interface IInvoiceService
{
    Task<IEnumerable<ServiceDto>> GetAllServicesAsync();
    Task<(bool success, ServiceDto? data, string? error)> GetServiceByIdAsync(string id);
    Task<ServiceDto> CreateServiceAsync(CreateServiceRequest request);
    Task<ServiceDto> UpdateServiceAsync(string id, UpdateServiceRequest request);
    Task DeleteServiceAsync(string id);
}