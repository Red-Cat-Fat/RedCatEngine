using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RedCatEngine.DependencyInjection.Containers.Attributes;
using RedCatEngine.DependencyInjection.Containers.Interfaces.Unity;
using RedCatEngine.DependencyInjection.Exceptions;
using RedCatEngine.DependencyInjection.Specials.Components;
using RedCatEngine.DependencyInjection.Specials.Providers;
using RedCatEngine.DependencyInjection.Utils;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RedCatEngine.DependencyInjection.Containers
{
	public class UnityGameContainer : ApplicationContainer, IUnityGameContainer
	{
		public MonoBehaviour MonoConstruct(MonoBehaviour monoBehaviour, params object[] context)
		{
			var type = monoBehaviour.GetType();
			var methods = type.GetMethods();
			foreach (var method in methods)
			{
				if (Attribute.GetCustomAttribute(
						method,
						typeof(MonoInjectAttribute),
						true) ==
					null)
					continue;

				return InjectContextToMonoConstructor(
					monoBehaviour,
					method,
					context);
			}

			throw new NotFountInjectAttributeForConstructorException(type);
		}

		public GameObject MonoConstruct(
			GameObject gameObject,
			bool constructChildren = false,
			params object[] context
		)
		{
			ConstructComponents(
				gameObject,
				constructChildren,
				context);

			return gameObject;
		}

		private void ConstructComponents(
			GameObject gameObject,
			bool constructChildren,
			object[] context
		)
		{
			var components = constructChildren
				? gameObject.GetComponentsInChildren(typeof(MonoConstruct))
					.Union(gameObject.GetComponents(typeof(MonoConstruct)))
				: gameObject.GetComponents(typeof(MonoConstruct));

			foreach (var component in components)
				MonoConstruct((MonoConstruct)component, context);
		}

		private MonoBehaviour InjectContextToMonoConstructor(
			MonoBehaviour monoBehaviour,
			MethodBase method,
			object[] context
		)
		{
			var parameters = new List<object>();

			foreach (var parameterInfo in method.GetParameters())
			{
				if (typeof(ISingleProvider<>).IsAssignableFromGeneric(
					parameterInfo.ParameterType,
					out var expectedSingleWaiterGenericType))
				{
					parameters.Add(_providerService.RegisterProvider(expectedSingleWaiterGenericType[0]));
					continue;
				}

				if (typeof(IArrayProvider<>).IsAssignableFromGeneric(
					parameterInfo.ParameterType,
					out var expectedArrayWaiterGenericType))
				{
					parameters.Add(_providerService.RegisterArrayProvider(expectedArrayWaiterGenericType[0]));
					continue;
				}

				parameters.Add(GetSingle(parameterInfo.ParameterType, context));
			}

			method.Invoke(monoBehaviour, parameters.ToArray());
			return monoBehaviour;
		}

		private MonoConstruct GetComponent(Type type, GameObject gameObject)
		{
			if (gameObject.TryGetComponent(type, out var component) && component is MonoConstruct monoConstruct)
				return monoConstruct;

			throw new GameObjectNotContainComponentException(type, gameObject);
		}

		private TMonoBehaviour GetComponent<TMonoBehaviour>(GameObject gameObject)
			where TMonoBehaviour : MonoConstruct
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
				true,
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
				true,
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
				true,
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
		) where TBindType : MonoConstruct
		{
			var go = Object.Instantiate(
				prefab,
				position,
				rotation,
				parent);

			ConstructComponents(
				go,
				false,
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
		) where TBindType : MonoConstruct
		{
			var go = Object.Instantiate(
				prefab,
				position,
				rotation,
				parent);

			ConstructComponents(
				go,
				false,
				context);
			var component = GetComponent<TBindType>(go);

			return BindAsArray(component);
		}

		public GameObject Create(
			GameObject prefab,
			Vector3 position,
			Quaternion rotation,
			Transform parent,
			bool constructChildren = false,
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
				constructChildren,
				context);
			return go;
		}

		public GameObject Create(
			GameObject prefab,
			Transform parent,
			bool constructChildren = false,
			params object[] context
		)
		{
			var go
				= Object.Instantiate(
					prefab,
					parent);;

			ConstructComponents(
				go,
				constructChildren,
				context);
			return go;
		}


		public TMonoBehaviorType CreateAndGetComponent<TMonoBehaviorType>(
			GameObject prefab,
			Transform parent,
			bool constructChildren = false,
			params object[] context
		) where TMonoBehaviorType : Component
		{
			return CreateAndGetComponent(
				typeof(TMonoBehaviorType),
				prefab,
				parent,
				constructChildren,
				context) as TMonoBehaviorType;
		}

		public TMonoBehaviorType CreateAndGetComponent<TMonoBehaviorType>(
			GameObject prefab,
			Vector3 position,
			Quaternion rotation,
			Transform parent,
			bool constructChildren = false,
			params object[] context
		) where TMonoBehaviorType : Component
		{
			return CreateAndGetComponent(
				typeof(TMonoBehaviorType),
				prefab,
				position,
				rotation,
				parent,
				constructChildren,
				context) as TMonoBehaviorType;
			//todo: error for not contain component
		}

		public object CreateAndGetComponent(
			Type componentType,
			GameObject prefab,
			Vector3 position,
			Quaternion rotation,
			Transform parent,
			bool constructChildren = false,
			params object[] context
		)
		{
			var go = Create(
				prefab,
				position,
				rotation,
				parent,
				constructChildren,
				context);

			return go.GetComponent(componentType);
		}

		public object CreateAndGetComponent(
			Type componentType,
			GameObject prefab,
			Transform parent,
			bool constructChildren = false,
			params object[] context
		)
		{
			var go = Create(
				prefab,
				parent,
				constructChildren,
				context);

			return go.GetComponent(componentType);
		}
	}
}