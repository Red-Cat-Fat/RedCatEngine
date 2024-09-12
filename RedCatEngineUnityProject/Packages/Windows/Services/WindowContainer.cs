using System;
using System.Collections.Generic;
using RedCatEngine.DependencyInjection.Containers.Attributes;
using RedCatEngine.DependencyInjection.Containers.Interfaces.Unity;
using RedCatEngine.Windows.Interfaces;
using UnityEngine;

namespace RedCatEngine.Windows.Services
{
	public class WindowContainer : IWindowContainer
	{
		private readonly IUnityGameContainer _windowContainerImplementation;

		[Inject]
		public WindowContainer(IUnityGameContainer windowContainerImplementation)
		{
			_windowContainerImplementation = windowContainerImplementation;
		}

		public bool TryGetSingle<T>(out T data)
		{
			return _windowContainerImplementation.TryGetSingle(out data);
		}

		public bool TryGetArray<T>(out IEnumerable<T> data)
		{
			return _windowContainerImplementation.TryGetArray(out data);
		}

		public T GetSingle<T>(params object[] context)
		{
			return _windowContainerImplementation.GetSingle<T>(context);
		}

		public object GetSingle(Type type, params object[] context)
		{
			return _windowContainerImplementation.GetSingle(type, context);
		}

		public IEnumerable<T> GetArray<T>()
		{
			return _windowContainerImplementation.GetArray<T>();
		}

		public object Create(Type type, params object[] context)
		{
			return _windowContainerImplementation.Create(type, context);
		}

		public T Create<T>(params object[] context)
		{
			return _windowContainerImplementation.Create<T>(context);
		}

		public GameObject Create(
			GameObject prefab,
			Vector3 position = default,
			Quaternion rotation = default,
			Transform parent = null,
			bool constructChildren = false,
			params object[] context
		)
		{
			return _windowContainerImplementation.Create(prefab,
				position,
				rotation,
				parent,
				constructChildren,
				context);
		}

		public GameObject Create(
			GameObject prefab,
			Transform parent = null,
			bool constructChildren = false,
			params object[] context
		)
		{
			return _windowContainerImplementation.Create(prefab,
				parent,
				constructChildren,
				context);
		}

		public TBindType CreateAndGetComponent<TBindType>(
			GameObject prefab,
			Vector3 position = default,
			Quaternion rotation = default,
			Transform parent = null,
			bool constructChildren = false,
			params object[] context
		) where TBindType : Component
		{
			return _windowContainerImplementation.CreateAndGetComponent<TBindType>(prefab,
				position,
				rotation,
				parent,
				constructChildren,
				context);
		}

		public object CreateAndGetComponent(
			Type componentType,
			GameObject prefab,
			Transform parent = null,
			bool constructChildren = false,
			params object[] context
		)
		{
			return _windowContainerImplementation.CreateAndGetComponent(componentType,
				prefab,
				parent,
				constructChildren,
				context);
		}

		public TBindType CreateAndGetComponent<TBindType>(
			GameObject prefab,
			Transform parent,
			bool constructChildren = false,
			params object[] context
		) where TBindType : Component
		{
			return _windowContainerImplementation.CreateAndGetComponent<TBindType>(prefab,
				parent,
				constructChildren,
				context);
		}

		public object CreateAndGetComponent(
			Type componentType,
			GameObject prefab,
			Vector3 position = default,
			Quaternion rotation = default,
			Transform parent = null,
			bool constructChildren = false,
			params object[] context
		)
		{
			return _windowContainerImplementation.CreateAndGetComponent(componentType,
				prefab,
				position,
				rotation,
				parent,
				constructChildren,
				context);
		}
	}
}