using System;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace SIO.Infrastructure.EntityFrameworkCore.SqlServer
{
    public class SIOEntityFrameworkCoreSqlServerOptions
    {
        internal string StoreConnectionString { get; private set; }
        internal string ProjectionConnectionString { get; private set; }
        internal Action<SqlServerDbContextOptionsBuilder> StoreOptions { get; private set; }
        internal Action<SqlServerDbContextOptionsBuilder> ProjectionOptions { get; private set; }

        public SIOEntityFrameworkCoreSqlServerOptions AddStore(string connectionString, Action<SqlServerDbContextOptionsBuilder> action = null)
        {
            StoreConnectionString = connectionString;
            StoreOptions = action;
            return this;
        }

        public SIOEntityFrameworkCoreSqlServerOptions AddProjections(string connectionString, Action<SqlServerDbContextOptionsBuilder> action = null)
        {
            ProjectionConnectionString = connectionString;
            ProjectionOptions = action;
            return this;
        }
    }
}
