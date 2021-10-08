using Microsoft.EntityFrameworkCore;

namespace SIO.Infrastructure.EntityFrameworkCore.EntityConfiguration
{
    public interface IProjectionTypeConfiguration<T> : IEntityTypeConfiguration<T>
        where T : class
    {
    }
}
