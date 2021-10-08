using SIO.Infrastructure.Projections;

namespace SIO.Infrastructure.EntityFrameworkCore.Projections
{
    public sealed class StoreProjectorOptions<TProjection>
        where TProjection : class, IProjection
    {
        public StoreProjectorOptions()
        {

        }

        public int Interval { get; set; }
    }
}
