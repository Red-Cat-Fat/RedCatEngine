﻿using System;
using NUnit.Framework;
using RedCatEngine.DependencyInjection.Containers;
using RedCatEngine.DependencyInjection.Exceptions;
using RedCatEngine.DependencyInjection.Tests.SpecialSubClasses;

namespace RedCatEngine.DependencyInjection.Tests
{
	public class ExceptionDiContainerTests
	{
		private IApplicationContainer _applicationContainer;

		[SetUp]
		public void SetUp()
		{
			_applicationContainer = new ServiceLocatorApplicationContainer();
		}

		[Test]
		public void GivenServiceLocatorContainer_WhenGetNotContainInstance_ThenCatchNotFoundInstanceException()
		{
			try
			{
				_applicationContainer.GetSingle<SimpleDemoParentClass>();
			}
			catch (Exception exception)
			{
				Assert.IsTrue(exception is NotFoundInstanceException, "Incorrect error");
				Assert.IsTrue(((NotFoundInstanceException)exception).NotFoundType == typeof(SimpleDemoParentClass), "Incorrect type");
			}
		}

		[Test]
		public void GivenServiceLocatorContainer_WhenGetAllNotContainInstance_ThenCatchNotFoundInstanceException()
		{
			try
			{
				_applicationContainer.GetArray<SimpleDemoParentClass>();
			}
			catch (Exception exception)
			{
				Assert.IsTrue(exception is NotFoundInstanceException, "Incorrect error");
				Assert.IsTrue(((NotFoundInstanceException)exception).NotFoundType == typeof(SimpleDemoParentClass), "Incorrect type");
			}
		}

		[Test]
		public void GivenServiceLocatorContainer_WhenAddDuplicateAsSingle_ThenCatchBindDuplicateWithoutArrayMarkException()
		{
			var applicationContainer = new ServiceLocatorApplicationContainer();
			applicationContainer.BindAsSingle(new SimpleDemoDataParentClass(42));
			try
			{
				applicationContainer.BindAsSingle(new SimpleDemoDataParentClass(24));
			}
			catch (Exception exception)
			{
				Assert.IsTrue(exception is BindDuplicateWithoutArrayMarkException, "Incorrect error");
				Assert.IsTrue(((BindDuplicateWithoutArrayMarkException)exception).DuplicateType == typeof(SimpleDemoDataParentClass), "Incorrect type");
				Assert.IsTrue(((SimpleDemoDataParentClass)((BindDuplicateWithoutArrayMarkException)exception).Instance).Value == 42, "Incorrect instance");
			}
		}
	}
}