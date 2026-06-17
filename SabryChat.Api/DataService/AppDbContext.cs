using System;
using System.Collections.Concurrent;
using SabryChat.Models;

namespace SabryChat.DataService;

using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<NotificationItem> Notifications { get; set; }
    public DbSet<OnlineUser> OnlineUsers { get; set; }  
    public DbSet<ChatMessage> ChatMessages { get;set;}
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasIndex(n => new { n.UserId, n.IsRead });
            entity.Property(n => n.Preview).HasMaxLength(500);
        });
        modelBuilder.Entity<OnlineUser>()
            .HasIndex(u => u.UserName)
            .IsUnique();
    }
}
