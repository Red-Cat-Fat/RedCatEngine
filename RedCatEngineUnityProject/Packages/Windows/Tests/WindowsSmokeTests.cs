using Infrastructure.Windows.Services;
using NUnit.Framework;

namespace RedCatEngine.Windows.Tests
{
    public class WindowsSmokeTests
    {
        private const string ExpectedAssemblyName = "MvpWindows";

        /// <summary>
        /// Unity's BuildPlayerDataGenerator (UnitySerializationLogic.ShouldNotTryToResolve) treats an assembly
        /// named exactly "Windows" as WinRT metadata and never resolves types from it. Fields of such types declared
        /// in other assemblies are dropped from the player TypeDB and the player crashes on startup
        /// ("sharedassets0.assets is corrupted! [Position out of bounds!]"). The name must never go back to "Windows".
        /// </summary>
        [Test]
        public void PackageAssemblyIsNotNamedWindows()
        {
            var assemblyName = typeof(WindowService).Assembly.GetName().Name;

            Assert.AreNotEqual("Windows", assemblyName);
        }

        [Test]
        public void PackageAssemblyHasExpectedName()
        {
            var assemblyName = typeof(WindowService).Assembly.GetName().Name;

            Assert.AreEqual(ExpectedAssemblyName, assemblyName);
        }
    }
}
