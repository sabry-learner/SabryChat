using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SabryChat.Models
{
    public class ChatMessage
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Room { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}