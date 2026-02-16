using System;
using System.Linq;
using NUnit.Framework;

namespace RedCatEngine.ApplicationRunner.Tests
{
    public class ApplicationRunnerSmokeTests
    {
        [Test]
        public void PackageAssemblyIsLoaded()
        {
            var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name == "ApplicationRunner");

            Assert.IsNotNull(assembly);
        }
    }
}
