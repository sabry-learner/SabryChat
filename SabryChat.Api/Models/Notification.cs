using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SabryChat.Models
{
    public class Notification
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string UserId { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string SenderName { get; set; } = string.Empty;
        public string Preview { get; set; } = string.Empty;
        public string ChatId { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public bool IsRead { get; set; } = false;
        public string? RelatedId { get; set; }
    }
}