using System;
using System.Collections.Generic;
using Infrastructure.Windows.Attributes;
using Infrastructure.Windows.Interfaces;
using RedCatEngine.DependencyInjection.Containers.Attributes;
using RedCatEngine.DependencyInjection.Containers.Interfaces.Unity;
using RedCatEngine.DependencyInjection.Specials;
using UnityEngine;

namespace RedCatEngine.Windows.Services
{
	public class WindowContainer : IWindowContainer
	{
		private readonly IUnityGameContainer _windowContainerImplementation;

		public Injector Injector
			=> _windowContainerImplementation.Injector;

		[Inject]
		public WindowContainer(IUnityGameContainer windowContainerImplementation)
		{
			_windowContainerImplementation = windowContainerImplementation;
		}

		public bool TryGetSingle<T>(out T data)
		{
			return _windowContainerImplementation.TryGetSingle(out data);
		}

		public bool TryGetSingle(Type type, out object data)
		{
			return _windowContainerImplementation.TryGetSingle(type, out data);
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
			Vector3 position,
			Quaternion rotation,
			Transform parent,
			params object[] context
		)
		{
			return _windowContainerImplementation.Create(
				prefab,
				position,
				rotation,
				parent,
				context);
		}

		public GameObject Create(
			GameObject prefab,
			Transform parent,
			params object[] context
		)
		{
			return _windowContainerImplementation.Create(
				prefab,
				parent,
				context);
		}

		public TBindType CreateAndGetComponent<TBindType>(
			GameObject prefab,
			Vector3 position,
			Quaternion rotation,
			Transform parent,
			params object[] context
		) where TBindType : Component
		{
			return _windowContainerImplementation.CreateAndGetComponent<TBindType>(
				prefab,
				position,
				rotation,
				parent,
				context);
		}

		public object CreateAndGetComponent(
			Type componentType,
			GameObject prefab,
			Transform parent,
			params object[] context
		)
		{
			return _windowContainerImplementation.CreateAndGetComponent(
				componentType,
				prefab,
				parent,
				context);
		}

		public TBindType CreateAndGetComponent<TBindType>(
			GameObject prefab,
			Transform parent,
			params object[] context
		) where TBindType : Component
		{
			return _windowContainerImplementation.CreateAndGetComponent<TBindType>(
				prefab,
				parent,
				context);
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
			return _windowContainerImplementation.CreateAndGetComponent(
				componentType,
				prefab,
				position,
				rotation,
				parent,
				context);
		}

		public IModel FillContextToModel(IModel model, params object[] context)
		{
			_windowContainerImplementation
				.Injector
				.InjectContextToMethodsWithAttribute<InjectModelContextAttribute>(
					model,
					context);
			return model;
		}
	}
}