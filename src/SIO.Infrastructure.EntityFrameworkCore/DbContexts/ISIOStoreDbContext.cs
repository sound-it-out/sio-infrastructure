using System;
using Microsoft.EntityFrameworkCore;
using SIO.Infrastructure.EntityFrameworkCore.Entities;
using SIO.Infrastructure.Events;
using Event = SIO.Infrastructure.EntityFrameworkCore.Entities.Event;

namespace SIO.Infrastructure.EntityFrameworkCore.DbContexts
{
    public interface ISIOStoreDbContext : IStoreContext, IDisposable
    {
        DbSet<Command> Commands { get; set; }
        DbSet<Event> Events { get; set; }
        DbSet<Query> Queries { get; set; }
    }
}
