using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIO.Infrastructure.EntityFrameworkCore.Entities;

namespace SIO.Infrastructure.EntityFrameworkCore.EntityConfiguration
{
    internal sealed class EventEntityTypeConfiguration : IEntityTypeConfiguration<Event>
    {
        public void Configure(EntityTypeBuilder<Event> builder)
        {
            builder.ToTable(name: nameof(Event), schema: "log");

            builder.HasKey(c => c.SequenceNo);
            builder.HasIndex(c => c.Id);
            builder.HasIndex(c => c.StreamId);
            builder.HasIndex(c => c.CorrelationId);
            builder.HasIndex(c => c.CausationId);
            builder.HasIndex(c => c.Name);
            builder.HasIndex(c => c.Actor);
        }
    }
}
