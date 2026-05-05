
using System.ComponentModel.DataAnnotations.Schema;

namespace AvalWebBackend.Domain.Entities
{
    public class Message
    {
        public string Id { get; set; } = string.Empty;
        public string TicketId { get; set; } = string.Empty;
        public string SenderId { get; set; } = string.Empty;
        public string SenderType { get; set; } = string.Empty;   

        [Column("Text")]           
        public string MessageText { get; set; } = string.Empty;

        public string Timestamp { get; set; } = string.Empty;
        public bool IsRead { get; set; }                         
    }
}