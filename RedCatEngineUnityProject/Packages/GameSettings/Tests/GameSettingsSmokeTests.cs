using System;
using System.Linq;
using NUnit.Framework;

namespace RedCatEngine.GameSettings.Tests
{
    public class GameSettingsSmokeTests
    {
        [Test]
        public void PackageAssemblyIsLoaded()
        {
            var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name == "GameSettings");

            Assert.IsNotNull(assembly);
        }
    }
}
