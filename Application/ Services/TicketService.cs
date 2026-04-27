using System.Globalization;
using System.Text.Json;
using AvalWebBackend.Application.Common.Interfaces;
using AvalWebBackend.Application.Common.Exceptions;
using AvalWebBackend.Application.DTOs;
using AvalWebBackend.Domain.Entities;

namespace AvalWebBackend.Application.Services;

public class TicketService : ITicketService
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IUserRepository _userRepository;

    public TicketService(ITicketRepository ticketRepository, IUserRepository userRepository)
    {
        _ticketRepository = ticketRepository;
        _userRepository = userRepository;
    }

    public async Task<IEnumerable<TicketDto>> GetAllTicketsAsync(string? userId)
    {
        var tickets = await _ticketRepository.GetAllAsync();
        if (!string.IsNullOrEmpty(userId))
            tickets = tickets.Where(t => t.UserId == userId).ToList();
        return tickets.Select(MapToDto);
    }

    public async Task<TicketDto> GetTicketByIdAsync(string id)
    {
        var ticket = await _ticketRepository.GetByIdAsync(id);
        if (ticket == null)
            throw new NotFoundException(nameof(Ticket), id);
        return MapToDto(ticket);
    }

    public async Task<TicketDto> CreateTicketAsync(CreateTicketRequest request)
    {
        if (!await _userRepository.UserExistsByIdAsync(request.UserId))
            throw new BusinessRuleException("کاربر نامعتبر است");

        var ticket = new Ticket
        {
            Id = Guid.NewGuid().ToString()[..8],
            UserId = request.UserId,
            Title = request.Title,
            ShortDetail = request.ShortDetail,
            Description = request.Description,
            Date = GetCurrentPersianDate(),
            Time = GetCurrentTime(),
            Status = "pending",
            File = request.File != null ? JsonSerializer.Serialize(request.File) : null   // serialise object to JSON string
        };

        await _ticketRepository.AddAsync(ticket);
        await _ticketRepository.SaveChangesAsync();

        return MapToDto(ticket);
    }

    public async Task<TicketDto> UpdateTicketAsync(string id, UpdateTicketRequest request)
    {
        var ticket = await _ticketRepository.GetByIdAsync(id);
        if (ticket == null)
            throw new NotFoundException(nameof(Ticket), id);

        ticket.Title = request.Title;
        ticket.ShortDetail = request.ShortDetail;
        ticket.Description = request.Description;
        ticket.Status = request.Status;
        ticket.AdminResponse = request.AdminResponse;

        if (request.File != null)
            ticket.File = JsonSerializer.Serialize(request.File);   // serialise object to JSON string

        await _ticketRepository.UpdateAsync(ticket);
        await _ticketRepository.SaveChangesAsync();

        return MapToDto(ticket);
    }

    public async Task DeleteTicketAsync(string id)
    {
        var ticket = await _ticketRepository.GetByIdAsync(id);
        if (ticket == null)
            throw new NotFoundException(nameof(Ticket), id);

        await _ticketRepository.DeleteAsync(id);
        await _ticketRepository.SaveChangesAsync();
    }

    public async Task<(byte[] fileBytes, string contentType, string fileName)> GetTicketFileAsync(string id)
    {
        var ticket = await _ticketRepository.GetByIdWithFileAsync(id);
        if (string.IsNullOrEmpty(ticket?.File))
            throw new NotFoundException(nameof(Ticket), id);

        // File is stored as a JSON string: {"name":"...","type":"...","data":"..."}
        JsonDocument document;
        try
        {
            document = JsonDocument.Parse(ticket.File);
        }
        catch
        {
            throw new BusinessRuleException("ساختار فایل نامعتبر است");
        }

        var root = document.RootElement;
        if (!root.TryGetProperty("data", out var dataElement) ||
            !root.TryGetProperty("name", out var nameElement) ||
            !root.TryGetProperty("type", out var typeElement))
            throw new BusinessRuleException("ساختار فایل نامعتبر است");

        var base64WithPrefix = dataElement.GetString();
        var fileName = nameElement.GetString();
        var contentType = typeElement.GetString();

        if (string.IsNullOrEmpty(base64WithPrefix))
            throw new BusinessRuleException("داده فایل خالی است");

        var base64Data = base64WithPrefix.Contains(',')
            ? base64WithPrefix[(base64WithPrefix.IndexOf(',') + 1)..]
            : base64WithPrefix;

        var fileBytes = Convert.FromBase64String(base64Data);
        return (fileBytes, contentType!, fileName!);
    }

    private string GetCurrentPersianDate()
    {
        var pc = new PersianCalendar();
        var now = DateTime.Now;
        string engDate = $"{pc.GetYear(now):0000}/{pc.GetMonth(now):00}/{pc.GetDayOfMonth(now):00}";
        return ToPersianDigits(engDate);
    }

    private string GetCurrentTime() => DateTime.Now.ToString("HH:mm");

    private static string ToPersianDigits(string input)
    {
        var persianDigits = new[] { '۰', '۱', '۲', '۳', '۴', '۵', '۶', '۷', '۸', '۹' };
        char[] result = input.ToCharArray();
        for (int i = 0; i < result.Length; i++)
        {
            if (char.IsDigit(result[i]))
                result[i] = persianDigits[(int)char.GetNumericValue(result[i])];
        }
        return new string(result);
    }

    private static TicketDto MapToDto(Ticket ticket) => new()
    {
        Id = ticket.Id,
        UserId = ticket.UserId,
        Title = ticket.Title,
        ShortDetail = ticket.ShortDetail,
        Description = ticket.Description,
        Date = ticket.Date,
        Time = ticket.Time,
        Status = ticket.Status,
        AdminResponse = ticket.AdminResponse,
        File = ticket.File      // ticket.File is a string? and TicketDto.File is also a string? – fine
    };
}