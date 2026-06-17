using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SabryChat.Models
{
    public class OnlineUser
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string ChatRoom { get; set; } = string.Empty;
        public DateTime ConnectedAt { get; set; }
        public string ConnectionId { get; set; } = string.Empty;
    }
}