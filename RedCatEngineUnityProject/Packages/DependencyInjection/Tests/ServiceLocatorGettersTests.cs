using System.Linq;
using NUnit.Framework;
using RedCatEngine.DependencyInjection.Containers;
using RedCatEngine.DependencyInjection.Tests.SpecialSubClasses;

namespace RedCatEngine.DependencyInjection.Tests
{
	public class ServiceLocatorGettersTests
	{
		private IApplicationContainer _applicationContainer;

		[SetUp]
		public void SetUp()
		{
			_applicationContainer = new ServiceLocatorApplicationContainer();
		}

		[Test]
		public void GivenServiceLocatorContainer_WhenGetAndTryGetInvoke_ThenInstancesAreEquals()
		{
			_applicationContainer.BindAsSingle(new SimpleDemoDataParentClass(42));
			var justGetSingle = _applicationContainer.GetSingle<SimpleDemoDataParentClass>();

			if(!_applicationContainer.TryGetSingle<SimpleDemoDataParentClass>(out var tryGetSingle))
				Assert.Fail("Not can get in TryGetSingle");
			
			Assert.AreEqual(justGetSingle, tryGetSingle, "Not equal instances");
		}
		
		[Test]
		public void GivenServiceLocatorContainer_WhenGetAndTryGetInvokeByParentClass_ThenInstancesAreEquals()
		{
			_applicationContainer.BindAsSingle(new SimpleDemoFirstDataChildClass(42));
			var justGetSingle = _applicationContainer.GetSingle<SimpleDemoDataParentClass>();

			if(!_applicationContainer.TryGetSingle<SimpleDemoDataParentClass>(out var tryGetSingle))
				Assert.Fail("Not can get in TryGetSingle");
			
			Assert.AreEqual(justGetSingle, tryGetSingle, "Not equal instances");
		}
		
		[Test]
		public void GivenServiceLocatorContainer_WhenGetArrayAndTryGetArrayInvoke_ThenInstancesAreEquals()
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
		public void GivenServiceLocatorContainer_WhenGetArrayAndTryGetArrayInvokeByParentClass_ThenInstancesAreEquals()
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
		public void GivenServiceLocatorContainerWithSingleBinds_WhenGetArrayAndTryGetArrayInvokeByParentClass_ThenInstancesAreEquals()
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