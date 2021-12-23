using TicketSystem.DiscordBot.IoC;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace TicketSystem.DiscordBot.Components
{
    [UsedImplicitly]
    public class ComponentServiceConfigurator : IServiceConfigurator
    {
        public void ConfigureServices(ServiceConfiguratorContext context)
        {
            context.ServiceCollection.AddSingleton<ComponentManager>();
        }
    }
}
