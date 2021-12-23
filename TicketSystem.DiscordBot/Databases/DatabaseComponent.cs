using TicketSystem.DiscordBot.Components;
using TicketSystem.DiscordBot.Databases.Tickets;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TicketSystem.DiscordBot.Databases
{
    [UsedImplicitly]
    public class DatabaseComponent : IComponent
    {
        private readonly ICollection<DbContext> _dbContexts;

        public DatabaseComponent(IServiceProvider serviceProvider)
        {
            _dbContexts = new DbContext[]
            {
                serviceProvider.GetRequiredService<TicketDbContext>()
            };
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            foreach (var dbContext in _dbContexts)
            {
                await dbContext.Database.MigrateAsync(cancellationToken);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
