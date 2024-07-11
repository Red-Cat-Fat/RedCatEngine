using System;
using NUnit.Framework;
using RedCatEngine.DependencyInjection.Containers;
using RedCatEngine.DependencyInjection.Containers.Interfaces;
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
			Assert.AreEqual(containData, dataClass, "Provider set, but not correct data class");
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
			Assert.AreEqual(containData, dataClass, "Provider set, but not correct data class");
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
	}
}