using System;
using RedCatEngine.DependencyInjection.Specials.Components;
using UnityEngine;

namespace RedCatEngine.DependencyInjection.Containers.Interfaces.Unity
{
	public interface IMonoBindInstance
	{
		GameObject BindInstance(
			GameObject prefab,
			Vector3 position,
			Quaternion rotation,
			Transform parent,
			params object[] context
		);

		object BindAsSingleInstance(
			Type bindType,
			GameObject prefab,
			Vector3 position,
			Quaternion rotation,
			Transform parent,
			params object[] context
		);

		object BindAsArrayInstance(
			Type bindType,
			GameObject prefab,
			Vector3 position,
			Quaternion rotation,
			Transform parent,
			params object[] context
		);

		TMonoConstruct BindAsSingleInstance<TMonoConstruct>(
			GameObject prefab,
			Vector3 position,
			Quaternion rotation,
			Transform parent,
			params object[] context
		) where TMonoConstruct : MonoConstruct;

		TMonoConstruct BindAsArrayInstance<TMonoConstruct>(
			GameObject prefab,
			Vector3 position,
			Quaternion rotation,
			Transform parent,
			params object[] context
		) where TMonoConstruct : MonoConstruct;
	}
}