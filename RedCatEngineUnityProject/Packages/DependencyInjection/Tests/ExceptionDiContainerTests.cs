using System;
using NUnit.Framework;
using RedCatEngine.DependencyInjection.Containers;
using RedCatEngine.DependencyInjection.Containers.Attributes;
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
				Assert.IsTrue(exception is NotFountInjectAttributeForConstructorException<InjectAttribute>, "Incorrect error");
				Assert.IsTrue(
					((NotFountInjectAttributeForConstructorException<InjectAttribute>)exception).NotFoundType == typeof(SimpleDemoSecondDataChildClass),
					"Incorrect type");
				return;
			}
			Assert.Fail("Not catch error");
		}

		[Test]
		public void GivenApplicationContainer_WhenGetAllNotContainInstance_ThenReturnEmptyArray()
		{
			var applicationContainer = new ApplicationContainer();
			var result = applicationContainer.GetArray<SimpleDemoParentClass>();

			Assert.IsEmpty(result, "GetArray should return empty array for unregistered type");
		}

		[Test]
		public void GivenApplicationContainer_WhenTryGetArrayNotContainInstance_ThenReturnsFalse()
		{
			var applicationContainer = new ApplicationContainer();
			var result = applicationContainer.TryGetArray<SimpleDemoParentClass>(out _);

			Assert.IsFalse(result, "TryGetArray should return false for unregistered type");
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