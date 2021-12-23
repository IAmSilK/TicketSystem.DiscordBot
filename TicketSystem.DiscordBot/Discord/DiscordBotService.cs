using System;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using Discord.Rest;
using TicketSystem.DiscordBot.Components;
using TicketSystem.DiscordBot.Discord.Commands;
using TicketSystem.DiscordBot.Tickets;

namespace TicketSystem.DiscordBot.Discord
{
    public class DiscordBotService : IHostedService
    {
        private readonly Runtime _runtime;
        private readonly ILogger<DiscordBotService> _logger;
        private readonly IConfiguration _configuration;
        private readonly CommandHandler _commandHandler;
        private readonly DiscordSocketClient _client;
        private readonly ComponentManager _componentManager;
        private readonly TicketService _ticketService;

        public DiscordBotService(Runtime runtime,
            ILogger<DiscordBotService> logger,
            IConfiguration configuration,
            CommandHandler commandHandler,
            DiscordSocketClient client,
            ComponentManager componentManager,
            TicketService ticketService)
        {
            _runtime = runtime;
            _logger = logger;
            _configuration = configuration;
            _commandHandler = commandHandler;
            _client = client;
            _componentManager = componentManager;
            _ticketService = ticketService;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var token = _configuration["DiscordToken"];

            if (string.IsNullOrWhiteSpace(token) || token == "CHANGEME")
            {
                _logger.LogCritical("A token must be specified in the config file.");

                // We must close the application by directly stopping the runtime
                // as if we call Environment.Exit, the method won't return until the
                // application has exited. The application however won't exit until this
                // method has returned resulting in the application locking.

                // ReSharper disable once MethodSupportsCancellation
                await _runtime.Host.StopAsync(cancellationToken);

                return;
            }

            await _componentManager.StartAsync(cancellationToken);

            _client.Log += OnLog;

            await _commandHandler.InstallCommandsAsync();

            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            _client.ReactionAdded += OnReactionAdded;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _client.StopAsync();

            _client.Log -= OnLog;

            await _componentManager.StopAsync(cancellationToken);
        }

        private Task OnLog(LogMessage log)
        {
            var logLevel = log.Severity switch
            {
                LogSeverity.Verbose => LogLevel.Debug,
                LogSeverity.Debug => LogLevel.Debug,
                LogSeverity.Info => LogLevel.Information,
                LogSeverity.Warning => LogLevel.Warning,
                LogSeverity.Error => LogLevel.Error,
                LogSeverity.Critical => LogLevel.Critical,
                _ => LogLevel.None
            };

            if (log.Exception == null)
            {
                _logger.Log(logLevel, log.Message);
            }
            else
            {
                _logger.Log(logLevel, log.Exception, log.Message);
            }

            return Task.CompletedTask;
        }

        private async Task OnReactionAdded(Cacheable<IUserMessage, ulong> cacheableMessage, Cacheable<IMessageChannel, ulong> cacheableChannel, SocketReaction reaction)
        {
            if (reaction.Emote.Name != TicketService.TicketOpenEmote || reaction.UserId == _client.CurrentUser.Id)
            {
                return;
            }

            var message = await cacheableMessage.GetOrDownloadAsync();

            if (!message.IsPinned || message.Author.Id != _client.CurrentUser.Id)
            {
                return;
            }

            if (message.Author is not IGuildUser guildUser)
            {
                return;
            }

            var user = await guildUser.Guild.GetUserAsync(reaction.UserId) ??
                       throw new Exception("Could not find user");

            await message.RemoveReactionAsync(reaction.Emote, user.Id);

            await _ticketService.OpenTicket(guildUser.Guild, null, user);
        }
    }
}
