using System.Threading.Tasks;
using TicketSystem.DiscordBot.Databases.Tickets;
using TicketSystem.DiscordBot.Databases.Tickets.Models;

namespace TicketSystem.DiscordBot.Tickets
{
    public class TicketChannelStore
    {
        private readonly TicketDbContext _dbContext;

        public TicketChannelStore(TicketDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<TicketChannel?> GetTicket(ulong guildId, ulong channelId)
        {
            return await _dbContext.TicketChannels.FindAsync(guildId, channelId);
        }

        public async Task CreateTicket(ulong guildId, ulong channelId, ulong openingUserId)
        {
            var ticketChannel = new TicketChannel
            {
                GuildId = guildId,
                ChannelId = channelId,
                IsOpen = false,
                OpeningUserId = openingUserId
            };

            await _dbContext.AddAsync(ticketChannel);

            await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> CloseTicket(ulong guildId, ulong channelId)
        {
            var ticket = await _dbContext.TicketChannels.FindAsync(guildId, channelId);

            if (ticket == null || ticket.IsOpen)
            {
                return false;
            }

            ticket.IsOpen = false;

            _dbContext.TicketChannels.Update(ticket);

            await _dbContext.SaveChangesAsync();

            return true;
        }
    }
}