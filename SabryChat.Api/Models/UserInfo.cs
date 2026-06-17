using System;

namespace SabryChat.Models;

public class UserInfo
{
    public string UserName { get; set; } = string.Empty;
    public string ChatRoom { get; set; } = string.Empty;
    public DateTime ConnectedAt { get; set; }
}