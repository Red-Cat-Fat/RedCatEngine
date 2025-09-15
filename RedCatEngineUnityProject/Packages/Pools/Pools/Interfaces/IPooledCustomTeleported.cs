using UnityEngine;

namespace RedCatEngine.Pools.Pools.Interfaces
{
	public interface IPooledCustomTeleported : IPooledTeleportedLogic
	{
		void TeleportTo(Vector3 position, Quaternion rotation);
	}
}