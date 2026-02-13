using System;
using System.Linq;
using NUnit.Framework;

namespace RedCatEngine.UniversalServices.Tests
{
    public class UniversalServicesSmokeTests
    {
        [Test]
        public void PackageAssemblyIsLoaded()
        {
            var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name == "CommonServices");

            Assert.IsNotNull(assembly);
        }
    }
}
