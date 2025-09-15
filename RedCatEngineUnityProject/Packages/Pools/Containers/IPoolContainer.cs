using System;
using UnityEngine;

namespace RedCatEngine.Pools.Containers
{
	public interface IPoolContainer
	{
		void SetBeforeDisableCallback(Action<GameObject> callbackBeforeDisable);
		void SetAfterEnableCallback(Action<GameObject> callbackAfterEnable);
		void KillAll(Action<GameObject> callbackBeforeKill = null);
	}
}