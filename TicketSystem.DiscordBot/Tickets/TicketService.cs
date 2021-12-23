using Discord;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketSystem.DiscordBot.Tickets
{
    public class TicketService
    {
        private readonly DiscordSocketClient _discordClient;
        private readonly TicketCategoryStore _ticketCategoryStore;
        private readonly TicketChannelStore _ticketChannelStore;

        public TicketService(DiscordSocketClient discordClient,
            TicketCategoryStore ticketCategoryStore,
            TicketChannelStore ticketChannelStore)
        {
            _discordClient = discordClient;
            _ticketCategoryStore = ticketCategoryStore;
            _ticketChannelStore = ticketChannelStore;
        }

        private const string DefaultCategoryName = "tickets";

        private const string DefaultChannelNameFormat = "ticket-{0}";

        public const string TicketOpenEmote = "✅";

        private async Task<ICategoryChannel?> GetCategory(IGuild guild)
        {
            var categoryId = await _ticketCategoryStore.GetCategory(guild.Id);

            if (categoryId == null)
            {
                return null;
            }

            var categories = await guild.GetCategoriesAsync();

            return categories.FirstOrDefault(x => x.Id == categoryId.Value);
        }

        private async Task<ICategoryChannel> GetOrCreateCategory(IGuild guild)
        {
            var category = await GetCategory(guild);

            if (category != null)
            {
                return category;
            }

            category = await guild.CreateCategoryAsync(DefaultCategoryName);

            await category.AddPermissionOverwriteAsync(guild.EveryoneRole,
                new OverwritePermissions(viewChannel: PermValue.Deny));

            await _ticketCategoryStore.SetCategory(guild.Id, category.Id);

            return category;
        }

        private async Task<string> GetSafeChannelName(IGuild guild, string? name)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                var stringBuilder = new StringBuilder(name.Length);

                char? GetSafeCharacter(char c)
                {
                    if (c == '-' || c == '_')
                    {
                        return c;
                    }

                    if (char.IsLetterOrDigit(c))
                    {
                        return char.ToLower(c);
                    }

                    return null;
                }

                foreach (var chr in name)
                {
                    var safeChr = GetSafeCharacter(chr);

                    if (safeChr != null)
                    {
                        stringBuilder.Append(safeChr.Value);
                    }
                }

                name = stringBuilder.ToString();
            }
            else
            {
                var ticketNum = await _ticketCategoryStore.GetTicketCount(guild.Id);

                name = string.Format(DefaultChannelNameFormat, ticketNum + 1);
            }

            var i = 0;
            var origName = name;

            var textChannels = await guild.GetTextChannelsAsync();

            while (textChannels.Any(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                i++;
                name = $"{origName}-{i}";
            }

            return name;
        }

        private async Task<ITextChannel> CreateTextChannel(IGuild guild, string? name, ICategoryChannel category)
        {
            name = await GetSafeChannelName(guild, name);

            return await guild.CreateTextChannelAsync(name, properties => properties.CategoryId = category.Id);
        }

        public async Task<IMessageChannel> OpenTicket(IGuild guild, string? name, IGuildUser openingUser)
        {
            var category = await GetOrCreateCategory(guild);

            var channel = await CreateTextChannel(guild, name, category);

            await channel.AddPermissionOverwriteAsync(_discordClient.CurrentUser,
                new OverwritePermissions(sendMessages: PermValue.Allow, viewChannel: PermValue.Allow));

            await channel.AddPermissionOverwriteAsync(openingUser,
                new OverwritePermissions(
                    sendMessages: PermValue.Allow,
                    viewChannel: PermValue.Allow,
                    addReactions: PermValue.Allow,
                    attachFiles: PermValue.Allow,
                    readMessageHistory: PermValue.Allow));

            await _ticketCategoryStore.IncreaseTicketCount(guild.Id);

            await _ticketChannelStore.CreateTicket(channel.GuildId, channel.Id, openingUser.Id);

            var message = await channel.SendMessageAsync($"Your ticket has been opened here {openingUser.Mention}!");

            var _ = Task.Run(async () =>
            {
                await Task.Delay(20000);

                await message.DeleteAsync();
            });

            return channel;
        }

        public async Task<bool> CloseTicket(ITextChannel channel)
        {
            if (!await _ticketChannelStore.CloseTicket(channel.Guild.Id, channel.Id))
            {
                return false;
            }

            await channel.DeleteAsync();

            return true;
        }

        public async Task<bool> RenameTicket(ITextChannel channel, string newTitle)
        {
            if (await _ticketChannelStore.GetTicket(channel.GuildId, channel.Id) == null)
            {
                return false;
            }

            newTitle = await GetSafeChannelName(channel.Guild, newTitle);

            await channel.ModifyAsync(x => x.Name = newTitle);

            return true;
        }
    }
}
