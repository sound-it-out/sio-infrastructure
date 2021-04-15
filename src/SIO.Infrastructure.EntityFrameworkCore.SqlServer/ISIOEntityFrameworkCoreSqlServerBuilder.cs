using System;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace SIO.Infrastructure.EntityFrameworkCore.SqlServer
{
    public class SIOEntityFrameworkCoreSqlServerOptions
    {
        internal bool UseStore { get; private set; }
        internal bool UseProjections { get; private set; }
        internal Action<SqlServerDbContextOptionsBuilder> StoreOptions { get; private set; }
        internal Action<SqlServerDbContextOptionsBuilder> ProjectionOptions { get; private set; }

        public SIOEntityFrameworkCoreSqlServerOptions AddStore(Action<SqlServerDbContextOptionsBuilder> action = null)
        {
            UseStore = true;
            StoreOptions = action;

            return this;
        }

        public SIOEntityFrameworkCoreSqlServerOptions AddProjections(Action<SqlServerDbContextOptionsBuilder> action = null)
        {
            UseProjections = true;
            ProjectionOptions = action;

            return this;
        }
    }
}
