using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SabryChat.DataService;
using SabryChat.Models;

namespace SabryChat.Hubs;

public class NotificationHub : Hub
{
    private readonly AppDbContext _context;

    public NotificationHub(AppDbContext context)
    {
        _context = context;
    }

    public override async Task OnConnectedAsync()
    {
        var userName = Context.GetHttpContext()?.Request.Query["userName"].ToString();

        if (!string.IsNullOrEmpty(userName))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, userName);
        }

        await base.OnConnectedAsync();
    }

    public async Task SendNotification(
        string recipient,
        string sender,
        string message,
        string room)
    {
        var notification = new NotificationItem
        {
            UserName = recipient,
            SenderName = sender,
            Message = message,
            ChatRoom = room,
            Timestamp = DateTime.UtcNow,
            IsRead = false
        };

        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();

    
        await Clients.Group(recipient)
            .SendAsync("ReceiveNotification", notification);
    }
}