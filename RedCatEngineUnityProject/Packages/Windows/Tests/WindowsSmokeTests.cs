using System;
using System.Linq;
using NUnit.Framework;

namespace RedCatEngine.Windows.Tests
{
    public class WindowsSmokeTests
    {
        [Test]
        public void PackageAssemblyIsLoaded()
        {
            var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name == "Windows");

            Assert.IsNotNull(assembly);
        }
    }
}
