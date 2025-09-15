using RedCatEngine.DependencyInjection.Containers.Attributes;
using RedCatEngine.DependencyInjection.Containers.Interfaces.Unity;
using RedCatEngine.Pools.Containers.Creators.Factories;
using RedCatEngine.Pools.Containers.Creators.InstanceCreator;
using UnityEngine;

namespace RedCatEngine.CommonServices.Factories
{
	public class DiPoolInstanceCreatorFactory : IPoolInstanceCreatorFactory
	{
		private readonly IUnityGameContainer _container;

		[Inject]
		public DiPoolInstanceCreatorFactory(IUnityGameContainer container)
		{
			_container = container;
		}
		public IInstanceCreator Make(GameObject prefab, Transform parent)
			=> new DiInstanceCreator(_container, prefab, parent);
	}
}