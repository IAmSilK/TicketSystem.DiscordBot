using System.Threading.Tasks;

namespace TicketSystem.DiscordBot
{
    public class Program
    {
        public static async Task Main()
        {
            var runtime = new Runtime();
            
            await runtime.InitAsync();
        }
    }
}
