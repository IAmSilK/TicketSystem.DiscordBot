using System;
using System.Linq;
using System.Threading.Tasks;
using TicketSystem.DiscordBot.Databases.Tickets;

namespace TicketSystem.DiscordBot.Tickets
{
    public class TicketCategoryStore
    {
        private readonly TicketDbContext _dbContext;

        public TicketCategoryStore(TicketDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ulong?> GetCategory(ulong guildId)
        {
            var category = await _dbContext.TicketCategories.FindAsync(guildId);

            return category?.CategoryId;
        }

        public async Task SetCategory(ulong guildId, ulong categoryId)
        {
            var category = await _dbContext.TicketCategories.FindAsync(guildId);

            if (category == null)
            {
                category = new()
                {
                    GuildId = guildId,
                    CategoryId = categoryId,
                    TicketCount = 0
                };

                await _dbContext.AddAsync(category);
            }
            else
            {
                category.CategoryId = categoryId;

                _dbContext.Update(category);
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task<int> GetTicketCount(ulong guildId)
        {
            var category = await _dbContext.TicketCategories.FindAsync(guildId);

            return category?.TicketCount ?? 0;
        }

        public async Task IncreaseTicketCount(ulong guildId)
        {
            var category = await _dbContext.TicketCategories.FindAsync(guildId) ??
                           throw new Exception("Ticket category does not exist");

            category.TicketCount++;

            _dbContext.Update(category);

            await _dbContext.SaveChangesAsync();
        }
    }
}
