using System.ComponentModel.DataAnnotations;

namespace SabryChat.Models;

public class NotificationItem
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string UserName { get; set; } = string.Empty;        // Recipient
    public string SenderName { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string ChatRoom { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public bool IsRead { get; set; } = false;
}