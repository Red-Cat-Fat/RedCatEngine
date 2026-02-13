using System;
using System.Linq;
using NUnit.Framework;

namespace RedCatEngine.Benchmark.Tests
{
    public class BenchmarkSmokeTests
    {
        [Test]
        public void PackageAssemblyIsLoaded()
        {
            var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name == "Benchmark");

            Assert.IsNotNull(assembly);
        }
    }
}
