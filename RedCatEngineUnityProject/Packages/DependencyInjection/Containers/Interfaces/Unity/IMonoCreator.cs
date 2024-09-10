using System;
using UnityEngine;

namespace RedCatEngine.DependencyInjection.Containers.Interfaces.Unity
{
	public interface IMonoCreator
	{
		GameObject Create(
			GameObject prefab,
			Transform parent,
			bool constructChildren = false,
			params object[] context
		);

		GameObject Create(
			GameObject prefab,
			Vector3 position,
			Quaternion rotation,
			Transform parent,
			bool constructChildren = false,
			params object[] context
		);

		TBindType CreateAndGetComponent<TBindType>(
			GameObject prefab,
			Transform parent,
			bool constructChildren = false,
			params object[] context
		) where TBindType : Component;

		TBindType CreateAndGetComponent<TBindType>(
			GameObject prefab,
			Vector3 position,
			Quaternion rotation,
			Transform parent,
			bool constructChildren = false,
			params object[] context
		) where TBindType : Component;

		object CreateAndGetComponent(
			Type componentType,
			GameObject prefab,
			Transform parent,
			bool constructChildren = false,
			params object[] context
		);
		object CreateAndGetComponent(
			Type componentType,
			GameObject prefab,
			Vector3 position,
			Quaternion rotation,
			Transform parent,
			bool constructChildren = false,
			params object[] context
		);
	}
}