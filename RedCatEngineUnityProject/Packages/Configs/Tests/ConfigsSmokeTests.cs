using System;
using System.Linq;
using NUnit.Framework;

namespace RedCatEngine.Configs.Tests
{
    public class ConfigsSmokeTests
    {
        [Test]
        public void PackageAssemblyIsLoaded()
        {
            var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name == "Configs");

            Assert.IsNotNull(assembly);
        }
    }
}
