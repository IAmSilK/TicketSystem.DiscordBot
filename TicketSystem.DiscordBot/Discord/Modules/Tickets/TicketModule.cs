using Discord;
using Discord.Commands;
using Discord.WebSocket;
using JetBrains.Annotations;
using System;
using System.Threading.Tasks;
using TicketSystem.DiscordBot.Discord.Commands;
using TicketSystem.DiscordBot.Tickets;

namespace TicketSystem.DiscordBot.Discord.Modules.Tickets
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class TicketModule : CustomModuleBase
    {
        private readonly TicketService _ticketService;

        public TicketModule(IServiceProvider serviceProvider,
            TicketService ticketService) : base(serviceProvider)
        {
            _ticketService = ticketService;
        }

        [Command("new")]
        [Alias("open")]
        public async Task New(string? name = null)
        {
            if (Context.User is not SocketGuildUser guildUser)
            {
                return;
            }

            var channel = await _ticketService.OpenTicket(Context.Guild, name, guildUser);

            var embed = new EmbedBuilder()
                .AddField("New Ticket Opened", $"Your new ticket has been opened at <#{channel.Id}>.")
                .Build();

            await ReplyAsync(embed: embed);
        }

        [Command("close")]
        public async Task Close()
        {
            if (Context.Channel is not SocketTextChannel socketTextChannel)
            {
                return;
            }

            await _ticketService.CloseTicket(socketTextChannel);
        }

        [Command("setup")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task Setup()
        {
            if (Context.Channel is not SocketTextChannel socketTextChannel)
            {
                return;
            }

            var message = await socketTextChannel.SendMessageAsync($"React with {TicketService.TicketOpenEmote} to open a new ticket!");

            await message.AddReactionAsync(new Emoji(TicketService.TicketOpenEmote));
            await message.PinAsync();
        }

        [Command("title")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task Setup(string title)
        {
            if (Context.Channel is not SocketTextChannel socketTextChannel)
            {
                return;
            }

            if (!await _ticketService.RenameTicket(socketTextChannel, title))
            {
                throw new UserFriendlyException("Unable to rename this channel. Is it an open ticket?");
            }

            await ReplyAndDeleteAsync($"Successfully renamed the channel <#{socketTextChannel.Id}>.");
        }
    }
}
