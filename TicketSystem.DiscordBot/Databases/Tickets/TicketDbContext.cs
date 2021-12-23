using Microsoft.EntityFrameworkCore;
using System;
using TicketSystem.DiscordBot.Databases.Tickets.Models;
using TicketSystem.DiscordBot.MySql;

namespace TicketSystem.DiscordBot.Databases.Tickets
{
    public class TicketDbContext : MySqlDbContext
    {
        public TicketDbContext(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public DbSet<TicketCategory> TicketCategories => Set<TicketCategory>();

        public DbSet<TicketChannel> TicketChannels => Set<TicketChannel>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TicketChannel>()
                .HasKey(x => new { x.GuildId, x.ChannelId });
        }
    }
}
