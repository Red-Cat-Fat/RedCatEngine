using NUnit.Framework;
using RedCatEngine.DependencyInjection.Containers;
using RedCatEngine.DependencyInjection.Containers.Interfaces.Unity;
using RedCatEngine.DependencyInjection.Specials.CreatedRule;
using UnityEngine;

namespace RedCatEngine.DependencyInjection.Tests
{
	public class ChildContainerTests
	{
		private IUnityGameContainer _applicationContainer;

		[SetUp]
		public void SetUp()
		{
			_applicationContainer = new UnityGameContainer(new UnityContainerModifyRule());
		}

		[Test]
		public void
			GivenApplicationContainerAndCreateChild_WhenAddChildTypeInChildContainer_ThenGetDataInChildIsCorrect()
		{
			const string childName = "Child";
			var childContainer = _applicationContainer
				.CreateChildContainer(
					childName,
					new UnityContainerModifyRule()
				);

			var getChildContainer = childContainer.GetSingle<IUnityGameContainer>();

			Debug.LogFormat(
				"Main container name: {1} Child name: {0}",
				getChildContainer.Name,
				_applicationContainer.Name
			);
			Assert.AreEqual("Main->Child", getChildContainer.Name);
		}

		[Test]
		public void
			GivenApplicationContainerAndCreateChild_WhenOverrideDataInChildContainer_ThenGetDataInChildIsCorrect()
		{
			const string childName = "Child";
			var childContainer = _applicationContainer
				.CreateChildContainer(
					childName,
					new UnityContainerModifyRule()
				);

			Assert.AreEqual("Main->Child", childContainer.Name);

			var instanceInMain = "Instance in main";
			var instanceInChild = "Instance in child";
			_applicationContainer.BindAsSingle(instanceInMain);
			childContainer.BindAsSingle(instanceInChild);
			
			Assert.AreNotEqual(_applicationContainer.GetSingle<string>(), childContainer.GetSingle<string>());
		}
		[Test]
		public void
			GivenApplicationContainerAndCreateChild_WhenNotOverrideDataInChildContainer_ThenGetDataInChildIsCorrect()
		{
			const string childName = "Child";
			var childContainer = _applicationContainer
				.CreateChildContainer(
					childName,
					new UnityContainerModifyRule()
				);
			
			Assert.AreEqual("Main->Child", childContainer.Name);

			var instanceInMain = "Instance in main";
			_applicationContainer.BindAsSingle(instanceInMain);
			
			Assert.AreEqual(instanceInMain, childContainer.GetSingle<string>());
		}
	}
}