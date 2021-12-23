using Autofac;
using Microsoft.Extensions.Configuration;

namespace TicketSystem.DiscordBot.IoC
{
    public class ContainerConfiguratorContext
    {
        public ContainerBuilder ContainerBuilder { get; }

        public IConfiguration Configuration { get; }

        public ContainerConfiguratorContext(ContainerBuilder containerBuilder, IConfiguration configuration)
        {
            ContainerBuilder = containerBuilder;
            Configuration = configuration;
        }
    }
}
