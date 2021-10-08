using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SIO.Infrastructure.Commands;
using SIO.Infrastructure.EntityFrameworkCore.DbContexts;
using SIO.Infrastructure.Serialization;

namespace SIO.Infrastructure.EntityFrameworkCore.Stores
{
    internal sealed class EntityFrameworkCoreCommandStore : ICommandStore
    {
        private readonly ISIOStoreDbContextFactory _dbContextFactory;
        private readonly ICommandSerializer _commandSerializer;
        private readonly ILogger<EntityFrameworkCoreCommandStore> _logger;

        public EntityFrameworkCoreCommandStore(ISIOStoreDbContextFactory dbContextFactory,
                                               ICommandSerializer commandSerializer,
                                               ILogger<EntityFrameworkCoreCommandStore> logger)
        {
            if (dbContextFactory == null)
                throw new ArgumentNullException(nameof(dbContextFactory));
            if (commandSerializer == null)
                throw new ArgumentNullException(nameof(commandSerializer));
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _dbContextFactory = dbContextFactory;
            _commandSerializer = commandSerializer;
            _logger = logger;
        }

        public async Task SaveAsync(ICommand command, CancellationToken cancellationToken = default)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            var type = command.GetType();
            var data = _commandSerializer.Serialize(command);

            using (var context = _dbContextFactory.Create())
            {
                await context.Commands.AddAsync(new Entities.Command
                {
                    Name = type.Name,
                    Type = type.FullName,
                    Subject = command.Subject,
                    CorrelationId = command.CorrelationId,
                    Data = data,
                    Id = command.Id,
                    UserId = command.Actor,
                    Timestamp = command.Timestamp,
                });

                await context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
