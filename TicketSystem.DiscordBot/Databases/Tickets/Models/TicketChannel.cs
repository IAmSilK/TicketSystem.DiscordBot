namespace TicketSystem.DiscordBot.Databases.Tickets.Models
{
    public class TicketChannel
    {
        public ulong GuildId { get; set; }

        public ulong ChannelId { get; set; }

        public bool IsOpen { get; set; }

        public ulong OpeningUserId { get; set; }
    }
}
