using System;

namespace TicketSystem.DiscordBot.Discord.Commands
{
    public class UserFriendlyException : Exception
    {
        public UserFriendlyException()
        {
        }

        public UserFriendlyException(string message) : base(message)
        {
        }
    }
}
