using System;
using System.Linq;
using NUnit.Framework;

namespace RedCatEngine.Rewards.Tests
{
    public class RewardsSmokeTests
    {
        [Test]
        public void PackageAssemblyIsLoaded()
        {
            var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name == "Rewards");

            Assert.IsNotNull(assembly);
        }
    }
}
