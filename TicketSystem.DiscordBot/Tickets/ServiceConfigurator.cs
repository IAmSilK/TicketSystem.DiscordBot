using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using TicketSystem.DiscordBot.IoC;

namespace TicketSystem.DiscordBot.Tickets
{
    [UsedImplicitly]
    internal class ServiceConfigurator : IServiceConfigurator
    {
        public void ConfigureServices(ServiceConfiguratorContext context)
        {
            context.ServiceCollection
                .AddTransient<TicketCategoryStore>()
                .AddTransient<TicketChannelStore>()
                .AddTransient<TicketService>();
        }
    }
}
