using System;
using System.Linq;
using RedCatEngine.DependencyInjection.Containers.Attributes;
using RedCatEngine.DependencyInjection.Containers.Interfaces.Application;
using RedCatEngine.DependencyInjection.Containers.Interfaces.Unity;
using RedCatEngine.DependencyInjection.Exceptions;
using RedCatEngine.DependencyInjection.Specials.Components;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RedCatEngine.DependencyInjection.Containers
{
	public class UnityGameContainer : ApplicationContainer, IUnityGameContainer
	{
		public UnityGameContainer()
		{
			
		}

		private UnityGameContainer(UnityGameContainer unityGameContainer) : base(unityGameContainer)
		{
		}

		public new IUnityGameContainer CreateChildContainer()
			=> new UnityGameContainer(this);

		public void MonoConstruct<TMonoBehaviour>(TMonoBehaviour monoBehaviour, params object[] context)
			where TMonoBehaviour : IMonoConstruct
		{
			Injector.InjectContextToMethodsWithAttribute<MonoInjectAttribute>(monoBehaviour, context);
			monoBehaviour.FinishInitialize();
		}

		public GameObject MonoConstruct(
			GameObject gameObject,
			params object[] context
		)
		{
			ConstructComponents(
				gameObject,
				context);

			return gameObject;
		}

		private void ConstructComponents(
			GameObject gameObject,
			object[] context
		)
		{
			var components = gameObject.GetComponentsInChildren(typeof(IMonoConstruct));

			foreach (var component in components.Cast<IMonoConstruct>())
				MonoConstruct(component, context);
		}


		private IMonoConstruct GetComponent(Type type, GameObject gameObject)
		{
			if (gameObject.TryGetComponent(type, out var component) && component is IMonoConstruct monoConstruct)
				return monoConstruct;

			throw new GameObjectNotContainComponentException(type, gameObject);
		}

		private TMonoBehaviour GetComponent<TMonoBehaviour>(GameObject gameObject)
			where TMonoBehaviour : IMonoConstruct
		{
			return (TMonoBehaviour)GetComponent(typeof(TMonoBehaviour), gameObject);
		}

		public GameObject BindInstance(
			GameObject prefab,
			Vector3 position,
			Quaternion rotation,
			Transform parent,
			params object[] context
		)
		{
			var go
				= Object.Instantiate(
					prefab,
					position,
					rotation,
					parent);

			ConstructComponents(
				go,
				context);

			return BindAsArray(go);
		}

		public object BindAsSingleInstance(
			Type bindType,
			GameObject prefab,
			Vector3 position,
			Quaternion rotation,
			Transform parent,
			params object[] context
		)
		{
			var go = Object.Instantiate(
				prefab,
				position,
				rotation,
				parent);

			ConstructComponents(
				go,
				context);
			var component = GetComponent(bindType, go);
			return BindAsSingle(bindType, component);
		}

		public object BindAsArrayInstance(
			Type bindType,
			GameObject prefab,
			Vector3 position,
			Quaternion rotation,
			Transform parent,
			params object[] context
		)
		{
			var go = Object.Instantiate(
				prefab,
				position,
				rotation,
				parent);

			ConstructComponents(
				go,
				context);
			var component = GetComponent(bindType, go);

			return BindAsArray(bindType, component);
		}

		public TBindType BindAsSingleInstance<TBindType>(
			GameObject prefab,
			Vector3 position,
			Quaternion rotation,
			Transform parent,
			params object[] context
		) where TBindType : IMonoConstruct
		{
			var go = Object.Instantiate(
				prefab,
				position,
				rotation,
				parent);

			ConstructComponents(
				go,
				context);
			var component = GetComponent<TBindType>(go);
			MonoConstruct(component, context);

			return BindAsSingle(component);
		}

		public TBindType BindAsArrayInstance<TBindType>(
			GameObject prefab,
			Vector3 position,
			Quaternion rotation,
			Transform parent,
			params object[] context
		) where TBindType : IMonoConstruct
		{
			var go = Object.Instantiate(
				prefab,
				position,
				rotation,
				parent);

			ConstructComponents(
				go,
				context);
			var component = GetComponent<TBindType>(go);

			return BindAsArray(component);
		}

		public GameObject Create(
			GameObject prefab,
			Vector3 position,
			Quaternion rotation,
			Transform parent,
			params object[] context
		)
		{
			var go
				= Object.Instantiate(
					prefab,
					position,
					rotation,
					parent);
			ConstructComponents(
				go,
				context);
			return go;
		}

		public GameObject Create(
			GameObject prefab,
			Transform parent,
			params object[] context
		)
		{
			var go
				= Object.Instantiate(
					prefab,
					parent);

			ConstructComponents(
				go,
				context);
			return go;
		}

		public TMonoBehaviorType CreateAndGetComponent<TMonoBehaviorType>(
			GameObject prefab,
			Transform parent,
			params object[] context
		) where TMonoBehaviorType : Component
		{
			return CreateAndGetComponent(
				typeof(TMonoBehaviorType),
				prefab,
				parent,
				context) as TMonoBehaviorType;
		}

		public TMonoBehaviorType CreateAndGetComponent<TMonoBehaviorType>(
			GameObject prefab,
			Vector3 position,
			Quaternion rotation,
			Transform parent,
			params object[] context
		) where TMonoBehaviorType : Component
		{
			return CreateAndGetComponent(
				typeof(TMonoBehaviorType),
				prefab,
				position,
				rotation,
				parent,
				context) as TMonoBehaviorType;
			//todo: error for not contain component
		}

		public TMonoBehaviorType CreateAndBindComponentAsSingle<TMonoBehaviorType>(
			GameObject prefab,
			Vector3 position,
			Quaternion rotation,
			Transform parent,
			params object[] context
		) where TMonoBehaviorType : Component
		{
			var go
				= Object.Instantiate(
					prefab,
					position,
					rotation,
					parent);

			var component = go.GetComponent<TMonoBehaviorType>();
			if (component is MonoConstruct monoConstruct)
				MonoConstruct(monoConstruct, context);

			BindAsSingle(component);
			ConstructComponents(
				go,
				context);

			return component;
		}

		public object CreateAndGetComponent(
			Type componentType,
			GameObject prefab,
			Vector3 position,
			Quaternion rotation,
			Transform parent,
			params object[] context
		)
		{
			var go = Create(
				prefab,
				position,
				rotation,
				parent,
				context);

			return go.GetComponent(componentType);
		}

		public object CreateAndGetComponent(
			Type componentType,
			GameObject prefab,
			Transform parent,
			params object[] context
		)
		{
			var go = Create(
				prefab,
				parent,
				context);

			return go.GetComponent(componentType);
		}
	}
}