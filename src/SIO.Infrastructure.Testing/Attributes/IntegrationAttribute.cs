using System;
using Xunit;
using Xunit.Sdk;

namespace SIO.Infrastructure.Testing.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    [TraitDiscoverer("SIO.Infrastructure.Testing.Discoverers.IntegrationTraitDiscoverer", "SIO.Infrastructure.Testing")]
    public class IntegrationAttribute : FactAttribute, ITraitAttribute { }
}
