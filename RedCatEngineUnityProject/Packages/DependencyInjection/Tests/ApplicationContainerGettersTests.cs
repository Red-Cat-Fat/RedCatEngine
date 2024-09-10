using System.Linq;
using NUnit.Framework;
using RedCatEngine.DependencyInjection.Containers;
using RedCatEngine.DependencyInjection.Containers.Interfaces.Application;
using RedCatEngine.DependencyInjection.Tests.SpecialSubClasses;

namespace RedCatEngine.DependencyInjection.Tests
{
	public class ApplicationContainerGettersTests
	{
		private IApplicationContainer _applicationContainer;

		[SetUp]
		public void SetUp()
		{
			_applicationContainer = new ApplicationContainer();
		}

		[Test]
		public void GivenApplicationContainer_WhenGetAndTryGetInvoke_ThenInstancesAreEquals()
		{
			_applicationContainer.BindAsSingle(new SimpleDemoDataParentClass(42));
			var justGetSingle = _applicationContainer.GetSingle<SimpleDemoDataParentClass>();

			if(!_applicationContainer.TryGetSingle<SimpleDemoDataParentClass>(out var tryGetSingle))
				Assert.Fail("Not can get in TryGetSingle");
			
			Assert.AreEqual(justGetSingle, tryGetSingle, "Not equal instances");
		}
		
		[Test]
		public void GivenApplicationContainer_WhenGetAndTryGetInvokeByParentClass_ThenInstancesAreEquals()
		{
			_applicationContainer.BindAsSingle(new SimpleDemoFirstDataChildClass(42));
			var justGetSingle = _applicationContainer.GetSingle<SimpleDemoDataParentClass>();

			if(!_applicationContainer.TryGetSingle<SimpleDemoDataParentClass>(out var tryGetSingle))
				Assert.Fail("Not can get in TryGetSingle");
			
			Assert.AreEqual(justGetSingle, tryGetSingle, "Not equal instances");
		}
		
		[Test]
		public void GivenApplicationContainer_WhenGetArrayAndTryGetArrayInvoke_ThenInstancesAreEquals()
		{
			_applicationContainer.BindAsArray(new SimpleDemoDataParentClass(42));
			_applicationContainer.BindAsArray(new SimpleDemoDataParentClass(43));
			var justGetArray = _applicationContainer.GetArray<SimpleDemoDataParentClass>().ToArray();

			if(!_applicationContainer.TryGetArray<SimpleDemoDataParentClass>(out var tryGetEnumerable))
				Assert.Fail("Not can get in TryGetSingle");

			var tryGetArray = tryGetEnumerable.ToArray();
			var intersect = justGetArray.Intersect(tryGetArray).ToArray();
			Assert.AreEqual(intersect.Length, justGetArray.Length, "Have you forgotten something");
			Assert.AreEqual(intersect.Length, tryGetArray.Length, "Have you forgotten something");
		}
		
		[Test]
		public void GivenApplicationContainer_WhenGetArrayAndTryGetArrayInvokeByParentClass_ThenInstancesAreEquals()
		{
			_applicationContainer.BindAsArray(new SimpleDemoFirstDataChildClass(42));
			_applicationContainer.BindAsArray(new SimpleDemoSecondDataChildClass(43));
			var justGetArray = _applicationContainer.GetArray<SimpleDemoDataParentClass>().ToArray();

			if(!_applicationContainer.TryGetArray<SimpleDemoDataParentClass>(out var tryGetEnumerable))
				Assert.Fail("Not can get in TryGetSingle");

			var tryGetArray = tryGetEnumerable.ToArray();
			var intersect = justGetArray.Intersect(tryGetArray).ToArray();
			Assert.AreEqual(intersect.Length, justGetArray.Length, "Have you forgotten something");
			Assert.AreEqual(intersect.Length, tryGetArray.Length, "Have you forgotten something");
		}

		[Test]
		public void GivenApplicationContainerWithSingleBinds_WhenGetArrayAndTryGetArrayInvokeByParentClass_ThenInstancesAreEquals()
		{
			_applicationContainer.BindAsSingle(new SimpleDemoFirstDataChildClass(42));
			_applicationContainer.BindAsSingle(new SimpleDemoSecondDataChildClass(43));
			var justGetArray = _applicationContainer.GetArray<SimpleDemoDataParentClass>().ToArray();

			if(!_applicationContainer.TryGetArray<SimpleDemoDataParentClass>(out var tryGetEnumerable))
				Assert.Fail("Not can get in TryGetSingle");

			var tryGetArray = tryGetEnumerable.ToArray();
			var intersect = justGetArray.Intersect(tryGetArray).ToArray();
			Assert.AreEqual(intersect.Length, justGetArray.Length, "Have you forgotten something");
			Assert.AreEqual(intersect.Length, tryGetArray.Length, "Have you forgotten something");
		}
	}
}