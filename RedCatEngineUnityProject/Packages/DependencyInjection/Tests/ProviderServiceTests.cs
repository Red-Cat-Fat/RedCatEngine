using System;
using NUnit.Framework;
using RedCatEngine.DependencyInjection.Containers;
using RedCatEngine.DependencyInjection.Containers.Interfaces;
using RedCatEngine.DependencyInjection.Containers.Interfaces.Application;
using RedCatEngine.DependencyInjection.Tests.SpecialSubClasses;

namespace RedCatEngine.DependencyInjection.Tests
{
	public class ProviderServiceTests
	{
		private IApplicationContainer _applicationContainer;

		[SetUp]
		public void SetUp()
		{
			_applicationContainer = new ApplicationContainer();
		}

		[Test]
		public void GivenSingleProvider_WhenGetSingleProviderAndBindAsSingleInstance_ThenProviderSetInstance()
		{
			var random = new Random();
			var dataClass = new SimpleDemoDataParentClass(random.Next());
			var provider = _applicationContainer.RegisterProvider<SimpleDemoDataParentClass>();
			_applicationContainer.BindAsSingle(dataClass);
			Assert.IsTrue(provider.TryGet(out var containData), "Provider not set data class");
			Assert.AreEqual(
				containData,
				dataClass,
				"Provider set, but not correct data class");
		}

		[Test]
		public void GivenSingleProvider_WhenGetSingleProviderAndBindAsArrayInstance_ThenProviderNotSetInstance()
		{
			var random = new Random();
			var dataClass = new SimpleDemoDataParentClass(random.Next());
			var provider = _applicationContainer.RegisterProvider<SimpleDemoDataParentClass>();
			_applicationContainer.BindAsArray(dataClass);
			Assert.IsFalse(provider.TryGet(out _), "Single Provider set data class after array bind");
		}

		[Test]
		public void GivenArrayProvider_WhenGetArrayProviderAndBindAsArrayInstance_ThenProviderSetInstance()
		{
			var random = new Random();
			var dataClass = new SimpleDemoDataParentClass(random.Next());
			var provider = _applicationContainer.RegisterArrayProvider<SimpleDemoDataParentClass>();
			_applicationContainer.BindAsArray(dataClass);
			Assert.IsTrue(provider.TryGet(out var containData), "Provider not set data class");
			Assert.AreEqual(
				containData[0],
				dataClass,
				"Provider set, but not correct data class");
		}

		[Test]
		public void GivenArrayProvider_WhenGetArrayProviderAndBindTwoClassAsArrayInstance_ThenProviderSetTwoInstance()
		{
			var random = new Random();
			var dataClass1 = new SimpleDemoDataParentClass(random.Next());
			var dataClass2 = new SimpleDemoDataParentClass(random.Next());
			var provider = _applicationContainer.RegisterArrayProvider<SimpleDemoDataParentClass>();
			_applicationContainer.BindAsArray(dataClass1);
			_applicationContainer.BindAsArray(dataClass2);
			Assert.IsTrue(provider.TryGet(out var containData), "Provider not set data class");
			Assert.IsTrue(containData.Length == 2, "Provider not set correct count data class");
			Assert.AreEqual(
				containData[0],
				dataClass1,
				"Provider set, but not correct data class");
			Assert.AreEqual(
				containData[1],
				dataClass2,
				"Provider set, but not correct data class");
		}

		[Test]
		public void GivenArrayProvider_WhenGetArrayProviderAndBindAsSingleInstance_ThenProviderNotSetInstance()
		{
			var random = new Random();
			var dataClass = new SimpleDemoDataParentClass(random.Next());
			var provider = _applicationContainer.RegisterArrayProvider<SimpleDemoDataParentClass>();
			_applicationContainer.BindAsSingle(dataClass);
			Assert.IsFalse(provider.TryGet(out _), "Provider set data class");
		}
		[Test]
		public void GivenArrayProvider_WhenBindArrayTypeUnderBaseType_ThenProviderSetInstance()
		{
			var provider = _applicationContainer.RegisterArrayProvider<SimpleDemoParentClass>();

			var instance = _applicationContainer.BindArrayType<SimpleDemoParentClass, SimpleDemoChildFirstClass>();

			Assert.IsTrue(provider.TryGet(out var containData), "Provider not set instance bound by BindArrayType");
			Assert.AreSame(instance, containData[0], "Provider set, but not correct instance");
		}

		[Test]
		public void GivenSingleProvider_WhenBindTypeByType_ThenProviderSetInstance()
		{
			var provider = _applicationContainer.RegisterProvider<SimpleDemoParentClass>();

			var instance = _applicationContainer.BindType(typeof(SimpleDemoParentClass));

			Assert.IsTrue(provider.TryGet(out var containData), "Provider not set instance bound by BindType(Type)");
			Assert.AreSame(instance, containData, "Provider set, but not correct instance");
		}

		[Test]
		public void GivenSingleProvider_WhenReBindAsSingle_ThenProviderSetNewInstance()
		{
			var random = new Random();
			var first = new SimpleDemoDataParentClass(random.Next());
			var second = new SimpleDemoDataParentClass(random.Next());
			var singleProvider = _applicationContainer.RegisterProvider<SimpleDemoDataParentClass>();
			var arrayProvider = _applicationContainer.RegisterArrayProvider<SimpleDemoDataParentClass>();

			_applicationContainer.BindAsSingle(first);
			_applicationContainer.ReBindAsSingle(second);

			Assert.IsTrue(singleProvider.TryGet(out var containData), "Provider lost instance after rebind");
			Assert.AreSame(second, containData, "Provider did not receive the rebound instance");
			Assert.IsFalse(arrayProvider.TryGet(out _), "Array provider received a single rebind");
		}
	}
}
