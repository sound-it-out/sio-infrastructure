using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIO.Infrastructure.EntityFrameworkCore.Entities;

namespace SIO.Infrastructure.EntityFrameworkCore.EntityConfiguration
{
    internal sealed class ProjectionStateEntityTypeConfiguration : IEntityTypeConfiguration<ProjectionState>
    {
        public void Configure(EntityTypeBuilder<ProjectionState> builder)
        {
            builder.ToTable(name: nameof(ProjectionState));

            builder.HasKey(c => c.Name);
        }
    }
}
