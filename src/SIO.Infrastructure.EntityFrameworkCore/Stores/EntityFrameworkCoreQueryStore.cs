using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SIO.Infrastructure.EntityFrameworkCore.DbContexts;
using SIO.Infrastructure.EntityFrameworkCore.Entities;
using SIO.Infrastructure.Queries;
using SIO.Infrastructure.Serialization;

namespace SIO.Infrastructure.EntityFrameworkCore.Stores
{
    internal sealed class EntityFrameworkCoreQueryStore : IQueryStore
    {
        private readonly ISIOStoreDbContextFactory _dbContextFactory;
        private readonly IQuerySerializer _querySerializer;
        private readonly ILogger<EntityFrameworkCoreQueryStore> _logger;

        public EntityFrameworkCoreQueryStore(ISIOStoreDbContextFactory dbContextFactory,
            IQuerySerializer querySerializer,
            ILogger<EntityFrameworkCoreQueryStore> logger)
        {
            if (dbContextFactory == null)
                throw new ArgumentNullException(nameof(dbContextFactory));
            if (querySerializer == null)
                throw new ArgumentNullException(nameof(querySerializer));
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _dbContextFactory = dbContextFactory;
            _querySerializer = querySerializer;
            _logger = logger;
        }

        public async Task SaveAsync<TQueryResult>(IQuery<TQueryResult> query, CancellationToken cancellationToken = default)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            var type = query.GetType();
            var data = _querySerializer.SerializeToJson(query);

            using (var context = _dbContextFactory.Create())
            {
                await context.Queries.AddAsync(new Query
                {
                    Name = type.Name,
                    Type = type.FullName,
                    Data = data,
                    Id = query.Id,
                    CorrelationId = query.CorrelationId,
                    Timestamp = query.Timestamp,
                    UserId = query.Actor,
                });

                await context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
