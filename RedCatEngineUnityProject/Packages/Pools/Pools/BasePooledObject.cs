using System;
using UnityEngine;

namespace RedCatEngine.Pools.Pools
{
	public abstract class BasePooledObject : MonoBehaviour, IPooledObject
	{
		public event Action<IPooledObject> DeadEvent;

		public GameObject GameObject
			=> gameObject;

		public void Enable()
		{
			DoEnable();
			gameObject.SetActive(true);
		}

		public void Disable()
		{
			DoDisable();
			if (this == null || gameObject == null)
				return;
			gameObject.SetActive(false);
			DeadEvent?.Invoke(this);
		}

		public void Reset()
			=> DoReset();

		public void TeleportTo(Vector3 position, Quaternion rotation)
		{
			DoDisableLogicBeforeTeleport();
			DoTeleportTo(position, rotation);
			DoEnableLogicAfterTeleport();
		}

		protected virtual void DoTeleportTo(Vector3 position, Quaternion rotation)
		{
			transform.position = position;
			transform.rotation = rotation;
		}

		protected virtual void DoEnable()
		{
		}

		protected virtual void DoDisable()
		{
		}

		protected virtual void DoReset()
		{
		}

		protected virtual void DoDisableLogicBeforeTeleport()
		{
		}

		protected virtual void DoEnableLogicAfterTeleport()
		{
		}
	}
}