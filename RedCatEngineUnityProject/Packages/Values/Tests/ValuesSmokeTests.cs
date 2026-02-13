using System;
using System.Linq;
using NUnit.Framework;

namespace RedCatEngine.Values.Tests
{
    public class ValuesSmokeTests
    {
        [Test]
        public void PackageAssemblyIsLoaded()
        {
            var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name == "Values");

            Assert.IsNotNull(assembly);
        }
    }
}
