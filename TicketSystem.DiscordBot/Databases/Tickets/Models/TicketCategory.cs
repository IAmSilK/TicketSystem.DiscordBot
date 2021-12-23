using System.ComponentModel.DataAnnotations;

namespace TicketSystem.DiscordBot.Databases.Tickets.Models
{
    public class TicketCategory
    {
        [Key]
        public ulong GuildId { get; set; }

        public ulong CategoryId { get; set; }

        public int TicketCount { get; set; }
    }
}
