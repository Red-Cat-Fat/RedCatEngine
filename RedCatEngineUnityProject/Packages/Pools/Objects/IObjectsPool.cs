using System;
using RedCatEngine.Pools.Pools;
using UnityEngine;

namespace RedCatEngine.Pools.Objects
{
	public interface IObjectsPool
	{
		GameObject Instantiate<TComponent>(out TComponent component)
			where TComponent : IPooledObject;

		void OnDead(IPooledObject pooledObject);
		void KillAll(Action<GameObject> callbackBeforeKill);
	}
}