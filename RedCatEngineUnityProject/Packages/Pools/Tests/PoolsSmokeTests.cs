using System;
using System.Linq;
using NUnit.Framework;

namespace RedCatEngine.Pools.Tests
{
    public class PoolsSmokeTests
    {
        [Test]
        public void PackageAssemblyIsLoaded()
        {
            var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name == "Pools");

            Assert.IsNotNull(assembly);
        }
    }
}
