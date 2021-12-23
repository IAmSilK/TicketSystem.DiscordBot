using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using TicketSystem.DiscordBot.Databases.Tickets;
using TicketSystem.DiscordBot.IoC;

namespace TicketSystem.DiscordBot.Databases
{
    [UsedImplicitly]
    public class DatabaseServiceConfigurator : IServiceConfigurator
    {
        public void ConfigureServices(ServiceConfiguratorContext context)
        {
            context.ServiceCollection.AddEntityFrameworkMySql()
                .AddDbContext<TicketDbContext>(ServiceLifetime.Transient);
        }
    }
}
