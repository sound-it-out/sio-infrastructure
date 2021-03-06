using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIO.Infrastructure.EntityFrameworkCore.Entities;

namespace SIO.Infrastructure.EntityFrameworkCore.EntityConfiguration
{
    internal class CommandEntityTypeConfiguration : IEntityTypeConfiguration<Command>
    {
        public void Configure(EntityTypeBuilder<Command> builder)
        {
            builder.ToTable(name: nameof(Command), schema: "log");

            builder.HasKey(c => c.SequenceNo);
            builder.HasIndex(c => c.Id);
            builder.HasIndex(c => c.Subject);
            builder.HasIndex(c => c.CorrelationId);
            builder.HasIndex(c => c.Name);
            builder.HasIndex(c => c.UserId);
        }
    }
}
