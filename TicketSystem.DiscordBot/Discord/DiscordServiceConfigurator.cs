using TicketSystem.DiscordBot.IoC;
using Discord.WebSocket;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace TicketSystem.DiscordBot.Discord
{
    [UsedImplicitly]
    public class DiscordServiceConfigurator : IServiceConfigurator
    {
        public void ConfigureServices(ServiceConfiguratorContext context)
        {
            context.ServiceCollection.AddSingleton(_ =>
                new DiscordSocketClient(new DiscordSocketConfig
                {
                    AlwaysDownloadUsers = true
                }));

            context.ServiceCollection.AddHostedService<DiscordBotService>();
        }
    }
}
