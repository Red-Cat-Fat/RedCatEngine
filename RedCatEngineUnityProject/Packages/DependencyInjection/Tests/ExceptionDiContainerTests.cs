using System;
using NUnit.Framework;
using RedCatEngine.DependencyInjection.Containers;
using RedCatEngine.DependencyInjection.Exceptions;
using RedCatEngine.DependencyInjection.Tests.SpecialSubClasses;

namespace RedCatEngine.DependencyInjection.Tests
{
	public class ExceptionDiContainerTests
	{

		[Test]
		public void GivenApplicationContainer_WhenGetNotContainInstance_ThenCatchNotFoundInstanceException()
		{
			var applicationContainer = new ApplicationContainer();
			try
			{
				applicationContainer.GetSingle<SimpleDemoSecondDataChildClass>();
			}
			catch (Exception exception)
			{
				Assert.IsTrue(exception is NotFountInjectAttributeForConstructorException, "Incorrect error");
				Assert.IsTrue(
					((NotFountInjectAttributeForConstructorException)exception).NotFoundType == typeof(SimpleDemoSecondDataChildClass),
					"Incorrect type");
				return;
			}
			Assert.Fail("Not catch error");
		}

		[Test]
		public void GivenApplicationContainer_WhenGetAllNotContainInstance_ThenCatchNotFoundInstanceException()
		{
			var applicationContainer = new ApplicationContainer();
			try
			{
				applicationContainer.GetArray<SimpleDemoParentClass>();
			}
			catch (Exception exception)
			{
				Assert.IsTrue(exception is NotFoundInstanceOrCreateException, "Incorrect error");
				Assert.IsTrue(
					((NotFoundInstanceOrCreateException)exception).NotFoundType == typeof(SimpleDemoParentClass),
					"Incorrect type");
				return;
			}
			Assert.Fail("Not catch error");
		}

		[Test]
		public void GivenApplicationContainer_WhenTryCreateInterface_ThenCatchNotCorrectType()
		{
			var applicationContainer = new ApplicationContainer();
			try
			{
				applicationContainer.GetArray<SimpleDemoParentClass>();
			}
			catch (Exception exception)
			{
				Assert.IsTrue(exception is NotFoundInstanceOrCreateException, "Incorrect error");
				Assert.IsTrue(
					((NotFoundInstanceOrCreateException)exception).NotFoundType == typeof(SimpleDemoParentClass),
					"Incorrect type");
				return;
			}
			Assert.Fail("Not catch error");
		}

		[Test]
		public void GivenApplicationContainer_WhenAddDuplicateAsSingle_ThenCatchBindDuplicateWithoutArrayMarkException()
		{
			var applicationContainer = new ApplicationContainer();
			applicationContainer.BindAsSingle(new SimpleDemoDataParentClass(42));
			try
			{
				applicationContainer.BindAsSingle(new SimpleDemoDataParentClass(24));
			}
			catch (Exception exception)
			{
				Assert.IsTrue(exception is BindDuplicateWithoutArrayMarkException, "Incorrect error");
				Assert.IsTrue(
					((BindDuplicateWithoutArrayMarkException)exception).DuplicateType ==
					typeof(SimpleDemoDataParentClass),
					"Incorrect type");
				Assert.IsTrue(
					((SimpleDemoDataParentClass)((BindDuplicateWithoutArrayMarkException)exception).Instance).Value ==
					42,
					"Incorrect instance");
				return;
			}
			Assert.Fail("Not catch error");
		}

		[Test]
		public void GivenApplicationContainer_WhenBindManyConstructorType_ThenCatchException()
		{
			var applicationContainer = new ApplicationContainer();
			applicationContainer.BindType<SimpleDemoParentClass, SimpleDemoChildFirstClass>();
			try
			{
				applicationContainer.BindType<SimpleDemoParentClass, SimpleDemoChildSecondClass>();
			}
			catch (Exception exception)
			{
				Assert.IsTrue(exception is BindDuplicateWithoutArrayMarkException, "Incorrect error");
				Assert.IsTrue(
					((BindDuplicateWithoutArrayMarkException)exception).DuplicateType ==
					typeof(SimpleDemoParentClass),
					"Incorrect type");
				return;
			}
			Assert.Fail("Not catch error");
		}
	}
}