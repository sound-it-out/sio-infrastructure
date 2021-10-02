using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SIO.Infrastructure.EntityFrameworkCore.DbContexts;
using SIO.Infrastructure.Projections;

namespace SIO.EntityFrameworkCore.Projections
{
    internal sealed class EntityFrameworkCoreProjectionWriter<TView> : IProjectionWriter<TView>
        where TView : class, IProjection
    {
        private readonly ISIOProjectionDbContextFactory  _projectionDbContextFactory;
        private readonly ILogger<EntityFrameworkCoreProjectionWriter<TView>> _logger;
        private readonly string _name;

        public EntityFrameworkCoreProjectionWriter(ISIOProjectionDbContextFactory projectionDbContextFactory,
            ILogger<EntityFrameworkCoreProjectionWriter<TView>> logger)
        {
            if (projectionDbContextFactory == null)
                throw new ArgumentNullException(nameof(projectionDbContextFactory));
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _projectionDbContextFactory = projectionDbContextFactory;
            _logger = logger;

            _name = typeof(TView).FullName;
        }

        public async Task<TView> AddAsync(string subject, Func<TView> add, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation($"{nameof(EntityFrameworkCoreProjectionWriter<TView>)}.{nameof(AddAsync)} was cancelled before execution");
                cancellationToken.ThrowIfCancellationRequested();
            }

            var entity = add();

            using (var context = _projectionDbContextFactory.Create())
            {
                await context.Set<TView>().AddAsync(entity);
                await context.SaveChangesAsync(cancellationToken);
            }

            return entity;
        }

        public async Task RemoveAsync(string subject, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation($"{nameof(EntityFrameworkCoreProjectionWriter<TView>)}.{nameof(RemoveAsync)} was cancelled before execution");
                cancellationToken.ThrowIfCancellationRequested();
            }

            using (var context = _projectionDbContextFactory.Create())
            {
                var entity = await context.Set<TView>().FindAsync(subject);

                if (entity == null)
                    return;

                context.Set<TView>().Remove(entity);

                await context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<TView> UpdateAsync(string subject, Func<TView, TView> update, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation($"{nameof(EntityFrameworkCoreProjectionWriter<TView>)}.{nameof(UpdateAsync)} was cancelled before execution");
                cancellationToken.ThrowIfCancellationRequested();
            }

            TView entity;

            using (var context = _projectionDbContextFactory.Create())
            {
                entity = await context.Set<TView>().FindAsync(subject);

                if (entity == null)
                    return null;

                update(entity);

                context.Set<TView>().Update(entity);

                await context.SaveChangesAsync(cancellationToken);
            }

            return entity;
        }

        public async Task<TView> UpdateAsync(string subject, Action<TView> update, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation($"{nameof(EntityFrameworkCoreProjectionWriter<TView>)}.{nameof(UpdateAsync)} was cancelled before execution");
                cancellationToken.ThrowIfCancellationRequested();
            }

            var view = await UpdateAsync(subject, v =>
            {
                update(v);
                return v;
            }, cancellationToken);

            return view;
        }

        public async Task ResetAsync(CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation($"{nameof(EntityFrameworkCoreProjectionWriter<TView>)}.{nameof(ResetAsync)} was cancelled before execution");
                cancellationToken.ThrowIfCancellationRequested();
            }

            using (var context = _projectionDbContextFactory.Create())
            {
                var projections = context.Set<TView>().ToArray();
                context.Set<TView>().RemoveRange(projections);

                var state = await context.ProjectionStates.FindAsync(_name);

                if (state != null)
                    context.ProjectionStates.Remove(state);

                await context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<TView> RetrieveAsync(string subject, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation($"{nameof(EntityFrameworkCoreProjectionWriter<TView>)}.{nameof(UpdateAsync)} was cancelled before execution");
                cancellationToken.ThrowIfCancellationRequested();
            }

            using (var context = _projectionDbContextFactory.Create())
            {
                return await context.Set<TView>().FindAsync(subject);
            }
        }
    }
}
