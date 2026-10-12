using NUnit.Framework;
using RedCatEngine.DependencyInjection.Containers;
using RedCatEngine.DependencyInjection.Exceptions;
using UnityEngine;

namespace RedCatEngine.DependencyInjection.Tests
{
	public class UnityObjectAutoCreateGuardTests
	{
		[Test]
		public void GivenContainer_WhenGetUnboundScriptableObject_ThenThrowsWithoutPoisonBind()
		{
			var applicationContainer = new ApplicationContainer();

			Assert.Throws<NotFoundInstanceOrCreateException>(() => applicationContainer.GetSingle<DemoScriptableConfig>()
			);

			var config = ScriptableObject.CreateInstance<DemoScriptableConfig>();
			Assert.DoesNotThrow(() => applicationContainer.BindAsSingle(config));
			Assert.AreSame(config, applicationContainer.GetSingle<DemoScriptableConfig>());
			Object.DestroyImmediate(config);
		}

		[Test]
		public void GivenContainer_WhenGetUnboundMonoBehaviour_ThenThrowsWithoutPoisonBind()
		{
			var applicationContainer = new ApplicationContainer();

			Assert.Throws<NotFoundInstanceOrCreateException>(() => applicationContainer.GetSingle<DemoMonoComponent>()
			);

			var gameObject = new GameObject(nameof(DemoMonoComponent));
			var component = gameObject.AddComponent<DemoMonoComponent>();
			Assert.DoesNotThrow(() => applicationContainer.BindAsSingle(component));
			Assert.AreSame(component, applicationContainer.GetSingle<DemoMonoComponent>());
			Object.DestroyImmediate(gameObject);
		}

		[Test]
		public void GivenContainer_WhenGetUnboundPlainClass_ThenAutoCreateStillWorks()
		{
			var applicationContainer = new ApplicationContainer();

			var created = applicationContainer.GetSingle<SpecialSubClasses.SimpleDemoParentClass>();

			Assert.IsNotNull(created);
			Assert.AreSame(created, applicationContainer.GetSingle<SpecialSubClasses.SimpleDemoParentClass>());
		}

		private class DemoScriptableConfig : ScriptableObject
		{
		}

		private class DemoMonoComponent : MonoBehaviour
		{
		}
	}
}
