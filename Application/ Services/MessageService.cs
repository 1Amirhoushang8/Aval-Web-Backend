using AvalWebBackend.Application.Common.Interfaces;
using AvalWebBackend.Application.Common.Exceptions;
using AvalWebBackend.Application.DTOs;
using AvalWebBackend.Domain.Entities;

namespace AvalWebBackend.Application.Services;

public class MessageService : IMessageService
{
    private readonly IMessageRepository _messageRepository;
    private readonly ITicketRepository _ticketRepository;
    private readonly IUserRepository _userRepository;

    public MessageService(IMessageRepository messageRepository,
                          ITicketRepository ticketRepository,
                          IUserRepository userRepository)
    {
        _messageRepository = messageRepository;
        _ticketRepository = ticketRepository;
        _userRepository = userRepository;
    }

    public async Task<IEnumerable<MessageDto>> GetMessagesByTicketAsync(string ticketId)
    {
        if (string.IsNullOrWhiteSpace(ticketId))
            throw new BusinessRuleException("شناسه تیکت الزامی است");

        var messages = await _messageRepository.GetByTicketIdAsync(ticketId);
        return messages.Select(MapToDto);
    }

    public async Task<MessageDto> SendMessageAsync(SendMessageRequest request)
    {
        if (!await _ticketRepository.ExistsAsync(request.TicketId))
            throw new BusinessRuleException("تیکت مورد نظر یافت نشد");

        bool isUser = await _userRepository.UserExistsByIdAsync(request.SenderId);
        bool isAdmin = await _userRepository.AdminExistsByIdAsync(request.SenderId);

        if (!isUser && !isAdmin)
            throw new BusinessRuleException("فرستنده نامعتبر است");

        var message = new Message
        {
            Id = Guid.NewGuid().ToString()[..8],
            TicketId = request.TicketId,
            SenderId = request.SenderId,
            MessageText = request.MessageText,
            Timestamp = DateTime.Now.ToString("yyyy/MM/dd HH:mm"),
            IsRead = false
        };

        await _messageRepository.AddAsync(message);
        await _messageRepository.SaveChangesAsync();

        return MapToDto(message);
    }

    public async Task<MessageDto> MarkAsReadAsync(string messageId)
    {
        var message = await _messageRepository.GetByIdAsync(messageId);
        if (message == null)
            throw new NotFoundException("پیام", messageId);

        message.IsRead = true;
        await _messageRepository.UpdateAsync(message);
        await _messageRepository.SaveChangesAsync();

        return MapToDto(message);
    }

    public async Task MarkMessagesAsReadAsync(string ticketId, string readerType)
    {
        if (string.IsNullOrWhiteSpace(ticketId))
            throw new BusinessRuleException("شناسه تیکت الزامی است");

        var messages = await _messageRepository.GetByTicketIdAsync(ticketId);
        if (!messages.Any())
            throw new BusinessRuleException("هیچ پیامی برای این تیکت یافت نشد");

        var messagesToUpdate = new List<Message>();
        foreach (var msg in messages)
        {
            if (readerType == "user" && await _userRepository.AdminExistsByIdAsync(msg.SenderId))
                messagesToUpdate.Add(msg);
            else if (readerType == "admin" && await _userRepository.UserExistsByIdAsync(msg.SenderId))
                messagesToUpdate.Add(msg);
        }

        if (!messagesToUpdate.Any())
            throw new BusinessRuleException("هیچ پیامی برای علامت‌گذاری وجود ندارد");

        foreach (var msg in messagesToUpdate)
        {
            msg.IsRead = true;
            await _messageRepository.UpdateAsync(msg);
        }

        await _messageRepository.SaveChangesAsync();
    }

    private static MessageDto MapToDto(Message message) => new()
    {
        Id = message.Id,
        TicketId = message.TicketId,
        SenderId = message.SenderId,
        MessageText = message.MessageText,
        Timestamp = message.Timestamp,
        IsRead = message.IsRead
    };
}