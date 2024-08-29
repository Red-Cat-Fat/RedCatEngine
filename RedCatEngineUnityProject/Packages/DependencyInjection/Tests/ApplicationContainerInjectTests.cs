using NUnit.Framework;
using RedCatEngine.DependencyInjection.Containers;
using RedCatEngine.DependencyInjection.Containers.Interfaces;
using RedCatEngine.DependencyInjection.Tests.SpecialSubClasses;
using UnityEngine;

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
		public void GivenApplicationContainer_WhenCreate_ThenInstancesAreEqualsType()
		{
			var simpleClass = _applicationContainer.Create<SimpleDemoParentClass>();
			Assert.IsTrue(simpleClass != null);
			var simpleClassObject = _applicationContainer.Create(typeof(SimpleDemoParentClass));
			Assert.IsTrue(simpleClassObject is SimpleDemoParentClass);
		}

		[Test]
		public void GivenApplicationContainer_WhenCreate_ThenInstancesAreEqualsAndNotSaveInContainer()
		{
			var simpleClass = _applicationContainer.Create<SimpleDemoParentClass>();
			Assert.IsTrue(simpleClass != null);
			var simpleClassObject = _applicationContainer.Create(typeof(SimpleDemoParentClass));
			Assert.IsTrue(simpleClassObject is SimpleDemoParentClass);
			Assert.IsFalse(_applicationContainer.TryGetSingle<SimpleDemoParentClass>(out _));
		}

		[Test]
		public void GivenApplicationContainer_WhenBindDefaultConstructorType_ThenCreateCorrect()
		{
			var simpleClass = _applicationContainer.BindType<SimpleDemoParentClass, SimpleDemoChildFirstClass>();
			Assert.IsTrue(simpleClass is SimpleDemoChildFirstClass, "Incorrect type");
		}

		[Test]
		public void GivenApplicationContainer_WhenBindDefaultConstructorType_ThenCreateCorrectAndSaveToContainer()
		{
			var simpleClass = _applicationContainer.BindType<SimpleDemoParentClass, SimpleDemoChildFirstClass>();
			Assert.IsTrue(simpleClass is SimpleDemoChildFirstClass, "Incorrect type");
			Assert.IsTrue(
				_applicationContainer.TryGetSingle<SimpleDemoParentClass>(out var saved),
				"Not found data in container");
			Assert.IsTrue(saved.Equals(simpleClass), "Data not equal type");
		}

		[Test]
		public void GivenApplicationContainer_WhenBindDefaultConstructorType_ThenInstanceSavedToContainer()
		{
			var simpleClass = _applicationContainer.BindType<SimpleDemoParentClass, SimpleDemoChildFirstClass>();
			Assert.IsTrue(simpleClass is SimpleDemoChildFirstClass);
			Assert.IsTrue(_applicationContainer.TryGetSingle<SimpleDemoParentClass>(out var result));
			Assert.IsTrue(simpleClass.Equals(result), "Not correct getting data");
			Assert.IsFalse(
				_applicationContainer.TryGetSingle<SimpleDemoChildFirstClass>(out _),
				"Instance bind as child type");
		}

		[Test]
		public void GivenApplicationContainer_WhenBindTypeWithDefaultConstructor_ThenInstanceSavedToContainer()
		{
			var simpleClass = _applicationContainer.BindType<SimpleDemoParentClass>();
			Assert.IsTrue(simpleClass != null);
			Assert.IsTrue(_applicationContainer.TryGetSingle<SimpleDemoParentClass>(out var savedClass));
			Assert.IsTrue(simpleClass.Equals(savedClass), "Incorrect save data");
		}

		[Test]
		public void GivenApplicationContainer_WhenBindNotDefaultConstructorType_ThenInstanceSavedToContainer()
		{
			var setData1 = Random.Range(0, 100000);
			var setData2 = Random.Range(0, 100000);
			var simpleClass = _applicationContainer.BindType<SimpleInjectedDemoParentClass>(
				new SimpleDemoFirstDataChildClass(setData1),
				new SimpleDemoSecondDataChildClass(setData2));
			Assert.IsTrue(simpleClass != null);
			Assert.IsTrue(_applicationContainer.TryGetSingle<SimpleInjectedDemoParentClass>(out var savedClass));
			Assert.IsTrue(simpleClass.Equals(savedClass), "Incorrect save data");
			Assert.IsTrue(savedClass.ContainData(), "Incorrect save data");
			Assert.IsTrue(savedClass.IsCorrect(setData1, setData2), "Incorrect save data");
		}

		[Test]
		public void GivenApplicationContainer_WhenBindNotDefaultConstructorType_WhitHalfConstructorDataInContainer_ThenInstanceSavedToContainer()
		{
			var setData1 = Random.Range(0, 100000);
			var setData2 = Random.Range(0, 100000);
			_applicationContainer.BindAsSingle(new SimpleDemoSecondDataChildClass(setData2));
			var simpleClass = _applicationContainer.BindType<SimpleInjectedDemoParentClass>(
				new SimpleDemoFirstDataChildClass(setData1));
			Assert.IsTrue(simpleClass != null);
			Assert.IsTrue(_applicationContainer.TryGetSingle<SimpleInjectedDemoParentClass>(out var savedClass));
			Assert.IsTrue(simpleClass.Equals(savedClass), "Incorrect save data");
			Assert.IsTrue(savedClass.ContainData(), "Incorrect save data");
			Assert.IsTrue(savedClass.IsCorrect(setData1, setData2), "Incorrect save data");
		}
	}
}