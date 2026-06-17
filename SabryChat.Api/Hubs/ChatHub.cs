using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SabryChat.DataService;
using SabryChat.Models;

namespace SabryChat.Hubs;

public class ChatHub : Hub
{
    private readonly AppDbContext _context;

    public ChatHub(AppDbContext context)
    {
        _context = context;
    }



    public async Task JoinChat(string userName, string room)
    {
        var existingUser = await _context.OnlineUsers
            .FirstOrDefaultAsync(x => x.UserName == userName);

        if (existingUser != null)
        {
            
            existingUser.ConnectionId = Context.ConnectionId;
            existingUser.ChatRoom = room;
            existingUser.ConnectedAt = DateTime.UtcNow;
        }
        else
        {
            _context.OnlineUsers.Add(new OnlineUser
            {
                UserName = userName,
                ConnectionId = Context.ConnectionId,
                ChatRoom = room,
                ConnectedAt = DateTime.UtcNow
            });
        }

        await _context.SaveChangesAsync();

        await Groups.AddToGroupAsync(Context.ConnectionId, room);

        await Clients.Group(room)
            .SendAsync("SystemMessage", $"{userName} joined {room}");

        await SendOnlineUsers(room);
    }

    public async Task SendMessage(string user, string room, string message)
    {

        _context.ChatMessages.Add(new ChatMessage
        {
            UserName = user,
            Room = room,
            Message = message,
            CreatedAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();

     
        await Clients.Group(room)
            .SendAsync("ReceiveMessage", user, message);

    
        await Clients.OthersInGroup(room)
            .SendAsync("ReceiveNotification", new
            {
                senderName = user,
                message
            });
    }


    public async Task LeaveChat(string userName, string room)
    {
        var user = await _context.OnlineUsers
            .FirstOrDefaultAsync(x => x.ConnectionId == Context.ConnectionId);

        if (user != null)
        {
            _context.OnlineUsers.Remove(user);
            await _context.SaveChangesAsync();
        }

        await Groups.RemoveFromGroupAsync(Context.ConnectionId, room);

        await Clients.Group(room)
            .SendAsync("SystemMessage", $"{userName} left {room}");

        await SendOnlineUsers(room);
    }

   
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var user = await _context.OnlineUsers
            .FirstOrDefaultAsync(x => x.ConnectionId == Context.ConnectionId);

        if (user != null)
        {
            var room = user.ChatRoom;

            _context.OnlineUsers.Remove(user);
            await _context.SaveChangesAsync();

            await SendOnlineUsers(room);
        }

        await base.OnDisconnectedAsync(exception);
    }
    private async Task SendOnlineUsers(string room)
    {
        var users = await _context.OnlineUsers
            .Where(x => x.ChatRoom == room)
            .Select(x => x.UserName)
            .Distinct()
            .ToListAsync();

        await Clients.Group(room)
            .SendAsync("UpdateUsers", users);
    }
}