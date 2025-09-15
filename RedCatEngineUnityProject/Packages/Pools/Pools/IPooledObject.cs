using System;
using UnityEngine;

namespace RedCatEngine.Pools.Pools
{
	public interface IPooledObject
	{
		event Action<IPooledObject> DeadEvent;
		GameObject GameObject { get; }
		void Enable();
		void Disable();
		void Reset();
		void TeleportTo(Vector3 position, Quaternion rotation);
	}
}