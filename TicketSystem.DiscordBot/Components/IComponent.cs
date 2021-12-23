using System.Threading;
using System.Threading.Tasks;

namespace TicketSystem.DiscordBot.Components
{
    public interface IComponent
    {
        Task StartAsync(CancellationToken cancellationToken);

        Task StopAsync(CancellationToken cancellationToken);
    }
}
