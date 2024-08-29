using NUnit.Framework;
using RedCatEngine.DependencyInjection.Containers;
using RedCatEngine.DependencyInjection.Containers.Interfaces;
using RedCatEngine.DependencyInjection.Tests.SpecialSubClasses;

namespace RedCatEngine.DependencyInjection.Tests
{
	public class ApplicationContainerInjectTests
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
			var simpleClass = _applicationContainer.Create<SimpleDemoParentClass>();
			Assert.IsTrue(simpleClass != null);
			var simpleClassObject = _applicationContainer.Create(typeof(SimpleDemoParentClass));
			Assert.IsTrue(simpleClassObject is SimpleDemoParentClass);
		}

		[Test]
		public void GivenApplicationContainer_WhenBindDefaultConstructorType_ThenCreateCorrect()
		{
			var simpleClass = _applicationContainer.BindType<SimpleDemoParentClass, SimpleDemoChildFirstClass>();
			Assert.IsTrue(simpleClass is SimpleDemoChildFirstClass);
		}

		[Test]
		public void GivenApplicationContainer_WhenBindDefaultConstructorType_ThenInstanceSavedToContainer()
		{
			var simpleClass = _applicationContainer.BindType<SimpleDemoParentClass, SimpleDemoChildFirstClass>();
			Assert.IsTrue(simpleClass is SimpleDemoChildFirstClass);
			Assert.IsTrue(_applicationContainer.TryGetSingle<SimpleDemoParentClass>(out var result));
			Assert.IsTrue(simpleClass.Equals(result), "Not correct getting data");
			Assert.IsFalse(
				_applicationContainer.TryGetSingle<SimpleDemoChildFirstClass>(out var resultChildren),
				"Instance bind as child type");
		}
	}
}