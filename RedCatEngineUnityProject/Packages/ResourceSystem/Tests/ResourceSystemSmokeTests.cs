using System;
using System.Linq;
using NUnit.Framework;

namespace RedCatEngine.ResourceSystem.Tests
{
    public class ResourceSystemSmokeTests
    {
        [Test]
        public void PackageAssemblyIsLoaded()
        {
            var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name == "Resources");

            Assert.IsNotNull(assembly);
        }
    }
}
