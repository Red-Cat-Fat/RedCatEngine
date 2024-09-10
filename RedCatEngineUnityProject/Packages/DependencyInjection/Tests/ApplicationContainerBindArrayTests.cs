using NUnit.Framework;
using RedCatEngine.DependencyInjection.Containers;
using RedCatEngine.DependencyInjection.Containers.Interfaces.Application;
using RedCatEngine.DependencyInjection.Tests.SpecialSubClasses;

namespace RedCatEngine.DependencyInjection.Tests
{
	public class ApplicationContainerBindArrayTests
	{
		private IApplicationContainer _applicationContainer;

		[SetUp]
		public void SetUp()
		{
			 _applicationContainer = new ApplicationContainer();
		}
		
		[Test]
		public void GivenApplicationContainer_WhenBindTwoChildAsArray_ThenCanGetArrayByParentType()
		{
			_applicationContainer.BindAsArray<SimpleDemoParentClass>(new SimpleDemoChildFirstClass());
			_applicationContainer.BindAsArray<SimpleDemoParentClass>(new SimpleDemoChildSecondClass());
			Assert.IsTrue(_applicationContainer.TryGetArray<SimpleDemoParentClass>(out _), "Not found instances");
		}

		[Test]
		public void GivenApplicationContainer_WhenBindTwoInstance_ThenGetCorrectArrayInstance()
		{
			const int checkValue = 42;
			_applicationContainer.BindAsArray(new SimpleDemoDataParentClass(checkValue));
			_applicationContainer.BindAsArray(new SimpleDemoDataParentClass(checkValue));
			var instances = _applicationContainer.GetArray<SimpleDemoDataParentClass>();
			foreach (var instance in instances)
			{
				Assert.AreEqual(
					instance.Value,
					checkValue,
					"Not correct instance");
			}
		}

		[Test]
		public void GivenApplicationContainer_WhenBindTwoChildAsArrayWithTyped_ThenCanNotGetArrayByParentType()
		{
			_applicationContainer.BindAsArray<SimpleDemoParentClass>(new SimpleDemoChildFirstClass());
			_applicationContainer.BindAsArray<SimpleDemoParentClass>(new SimpleDemoChildSecondClass());
			Assert.IsTrue(_applicationContainer.TryGetArray<SimpleDemoParentClass>(out _), "Not found instance");
			Assert.IsFalse(
				_applicationContainer.TryGetSingle<SimpleDemoParentClass>(out _),
				"Found array instance as single");
		}

		[Test]
		public void GivenApplicationContainer_WhenBindTwoChildAsArrayWithoutTyped_ThenCanGetArrayByParentTypeAndCanNotGetAsSingle()
		{
			_applicationContainer.BindAsArray(new SimpleDemoChildFirstClass());
			_applicationContainer.BindAsArray(new SimpleDemoChildSecondClass());
			Assert.IsTrue(_applicationContainer.TryGetArray<SimpleDemoParentClass>(out _), "Not found instance");
			Assert.IsFalse(
				_applicationContainer.TryGetSingle<SimpleDemoParentClass>(out _),
				"Found array instance as single");
		}

		[Test]
		public void GivenApplicationContainer_WhenBindTwoChildAsArray_ThenCanNotGetSingle()
		{
			_applicationContainer.BindAsArray<SimpleDemoParentClass>(new SimpleDemoChildFirstClass());
			_applicationContainer.BindAsArray<SimpleDemoParentClass>(new SimpleDemoChildSecondClass());
			Assert.IsTrue(_applicationContainer.TryGetArray<SimpleDemoParentClass>(out _), "Not found instances");
			Assert.IsFalse(_applicationContainer.TryGetSingle<SimpleDemoParentClass>(out _), "Found instances as single");
		}

		[Test]
		public void GivenApplicationContainer_WhenBindTwoChild_ThenCanNotGetArrayByParentType()
		{
			_applicationContainer.BindAsSingle(new SimpleDemoChildFirstClass());
			_applicationContainer.BindAsSingle(new SimpleDemoChildSecondClass());
			Assert.IsTrue(_applicationContainer.TryGetArray<SimpleDemoParentClass>(out _), "Not found instances");
		}
	}
}