using System;
using Xunit;
using Xunit.Sdk;

namespace SIO.Infrastructure.Testing.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    [TraitDiscoverer("SIO.Infrastructure.Testing.Discoverers.ThenTraitDiscoverer", "SIO.Infrastructure.Testing")]
    public class ThenAttribute : FactAttribute, ITraitAttribute { }
}
