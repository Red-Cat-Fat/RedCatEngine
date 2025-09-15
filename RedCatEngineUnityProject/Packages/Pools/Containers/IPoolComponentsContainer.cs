using System;
using System.Collections.Generic;
using RedCatEngine.Pools.Pools;
using UnityEngine;

namespace RedCatEngine.Pools.Containers
{
	public interface IPoolComponentsContainer<out TComponent> 
		: IEnumerable<TComponent>, IPoolContainer
		where TComponent : IPooledObject
	{
		TComponent this[int currentSelectIndex] { get; }
		int Length { get; }
		new IEnumerator<TComponent> GetEnumerator();
		TComponent InstantiateComponent(params object[] context);
		TComponent InstantiateComponent(Vector3 position, params object[] context);
		TComponent InstantiateComponentAsLastSibling(params object[] context);
		void KillAll(Action<TComponent> callbackBeforeKill);
		GameObject InstantiateAsLastSibling(params object[] context);
		GameObject Instantiate(params object[] context);
		GameObject Instantiate(
			Vector3 position,
			params object[] context
		);
		GameObject Instantiate(
			Vector3 position,
			Quaternion rotation,
			params object[] context
		);
	}
}