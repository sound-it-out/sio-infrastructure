using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Infrastructure;
using SIO.Infrastructure.EntityFrameworkCore.DbContexts;

namespace SIO.Infrastructure.EntityFrameworkCore.SqlServer
{
    public class SIOEntityFrameworkCoreSqlServerOptions
    {
        internal string ProjectionConnectionString { get; private set; }
        internal List<StoreOption> StoreOptions { get; private set; }
        internal Action<SqlServerDbContextOptionsBuilder> ProjectionOptions { get; private set; }

        public SIOEntityFrameworkCoreSqlServerOptions()
        {
            StoreOptions = new();
        }

        public SIOEntityFrameworkCoreSqlServerOptions AddStore<TStoreDbContext>(string connectionString, Action<SqlServerDbContextOptionsBuilder> action = null)
            where TStoreDbContext : ISIOStoreDbContext
        {
            StoreOptions.Add(new StoreOption(typeof(TStoreDbContext), connectionString, action));
            return this;
        }

        public SIOEntityFrameworkCoreSqlServerOptions AddProjections(string connectionString, Action<SqlServerDbContextOptionsBuilder> action = null)
        {
            ProjectionConnectionString = connectionString;
            ProjectionOptions = action;
            return this;
        }        
    }

    internal record StoreOption(Type StoreType, string ConnectionString, Action<SqlServerDbContextOptionsBuilder> StoreOptions);
}
