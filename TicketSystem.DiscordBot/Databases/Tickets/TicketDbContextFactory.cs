using JetBrains.Annotations;
using TicketSystem.DiscordBot.MySql;

namespace TicketSystem.DiscordBot.Databases.Tickets
{
    [UsedImplicitly]
    public class TicketDbContextFactory : MySqlDbContextFactory<TicketDbContext>
    {
    }
}
