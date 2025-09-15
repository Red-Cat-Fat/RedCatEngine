using System;
using RedCatEngine.Pools.Containers.Creators.Factories;
using RedCatEngine.Pools.Containers.KeyContainers;
using RedCatEngine.Pools.Containers.KeyContainers.Rules;
using RedCatEngine.Pools.Pools;
using UnityEngine;

namespace RedCatEngine.Pools.Serializable
{
	[Serializable]
	public class UnityKeyComponentsPoolSerializable<TKey, TComponent> : IKeyPoolComponentsContainer<TKey, TComponent>
		where TKey : IKeyRuleGameObjectSelector
		where TComponent : IPooledObject
	{
		[SerializeField] private Transform _parentTransform;

		private IKeyPoolComponentsContainer<TKey, TComponent> _hiddenPoolContainer;

		private IKeyPoolComponentsContainer<TKey, TComponent> Container
			=> _hiddenPoolContainer ??= new KeyPoolComponentsContainer<TKey, TComponent>(
				new SimpleUnityPoolInstanceCreatorFactory(),
				_parentTransform
			);

		public void SetBeforeDisableCallback(Action<GameObject> callbackBeforeDisable) =>
			Container.SetBeforeDisableCallback(callbackBeforeDisable);

		public void SetAfterEnableCallback(Action<GameObject> callbackAfterEnable) =>
			Container.SetAfterEnableCallback(callbackAfterEnable);

		public void KillAll(Action<GameObject> callbackBeforeKill = null)
			=> Container.KillAll(callbackBeforeKill);

		public GameObject Instantiate(TKey key, params object[] additionalContext) =>
			Container.Instantiate(key, additionalContext);

		public GameObject Instantiate(
			TKey key,
			Vector3 transformPosition,
			Quaternion transformRotation,
			params object[] additionalContext
		)
		{
			return Container.Instantiate(
				key,
				transformPosition,
				transformRotation,
				additionalContext
			);
		}

		public TInstanceComponent Instantiate<TInstanceComponent>(TKey key, params object[] additionalContext)
			where TInstanceComponent : IPooledObject
		{
			return Container.Instantiate<TInstanceComponent>(key, additionalContext);
		}

		public TComponent InstantiateComponent(TKey key, params object[] context) =>
			Container.InstantiateComponent(key, context);

		public void KillAll(Action<TComponent> callbackBeforeKill)
			=> Container.KillAll(callbackBeforeKill);
	}
}