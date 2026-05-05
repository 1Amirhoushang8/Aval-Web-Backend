using AvalWebBackend.Application.Common.Interfaces;
using AvalWebBackend.Application.Common.Exceptions;
using AvalWebBackend.Application.DTOs;
using AvalWebBackend.Domain.Entities;

namespace AvalWebBackend.Application.Services;

public class InvoiceService : IInvoiceService
{
    private readonly IServiceRepository _serviceRepository;
    private readonly IUserRepository _userRepository;

    public InvoiceService(IServiceRepository serviceRepository, IUserRepository userRepository)
    {
        _serviceRepository = serviceRepository;
        _userRepository = userRepository;
    }

    public async Task<IEnumerable<ServiceDto>> GetAllServicesAsync()
    {
        var services = await _serviceRepository.GetAllAsync();
        return services.Select(MapToDto);
    }

    public async Task<(bool success, ServiceDto? data, string? error)> GetServiceByIdAsync(string id)
    {
        var service = await _serviceRepository.GetByIdAsync(id);
        if (service == null) return (false, null, "سرویس یافت نشد");
        return (true, MapToDto(service), null);
    }

    public async Task<ServiceDto> CreateServiceAsync(CreateServiceRequest request)
    {
        // ✅ Use existing existence check
        if (!await _userRepository.UserExistsByIdAsync(request.UserId))
            throw new NotFoundException("کاربر", request.UserId);

        var service = new Service
        {
            Id = Guid.NewGuid().ToString()[..8],
            UserId = request.UserId,
            SerialNumber = request.SerialNumber,
            ServiceName = request.ServiceName,
            Price = request.Price,
            Status = request.Status,
            PaymentType = request.PaymentType,
            MonthlyPayment = request.MonthlyPayment,
            TotalMonths = request.TotalMonths
        };

        await _serviceRepository.AddAsync(service);
        await _serviceRepository.SaveChangesAsync();

        // 🔁 Re‑fetch with User included so the DTO gets the full name
        var savedService = await _serviceRepository.GetByIdAsync(service.Id);
        return MapToDto(savedService!);
    }

    public async Task<ServiceDto> UpdateServiceAsync(string id, UpdateServiceRequest request)
    {
        var service = await _serviceRepository.GetByIdAsync(id)
            ?? throw new NotFoundException("سرویس", id);

        if (request.SerialNumber != null) service.SerialNumber = request.SerialNumber;
        if (request.ServiceName != null) service.ServiceName = request.ServiceName;
        if (request.Price != null) service.Price = request.Price;
        if (request.Status != null) service.Status = request.Status;
        if (request.PaymentType != null) service.PaymentType = request.PaymentType;
        if (request.MonthlyPayment != null) service.MonthlyPayment = request.MonthlyPayment;
        if (request.TotalMonths.HasValue) service.TotalMonths = request.TotalMonths.Value;

        await _serviceRepository.UpdateAsync(service);
        await _serviceRepository.SaveChangesAsync();

        return MapToDto(service);
    }

    public async Task DeleteServiceAsync(string id)
    {
        var service = await _serviceRepository.GetByIdAsync(id)
            ?? throw new NotFoundException("سرویس", id);

        await _serviceRepository.DeleteAsync(id);
        await _serviceRepository.SaveChangesAsync();
    }

    private static ServiceDto MapToDto(Service service) => new()
    {
        Id = service.Id,
        UserId = service.UserId,
        UserFullName = service.User?.FullName,
        SerialNumber = service.SerialNumber,
        ServiceName = service.ServiceName,
        Price = service.Price,
        Status = service.Status,
        PaymentType = service.PaymentType,
        MonthlyPayment = service.MonthlyPayment,
        TotalMonths = service.TotalMonths
    };
}