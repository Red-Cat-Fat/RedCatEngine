using System;
using RedCatEngine.Pools.Pools.Interfaces;
using UnityEngine;

namespace RedCatEngine.Pools.Pools
{
	public class CollectorComponentsPoolObject : MonoBehaviour, IPooledObject
	{
		public event Action<IPooledObject> DeadEvent;

		private IPooledEnable[] _enabledComponents = Array.Empty<IPooledEnable>();
		private IPooledDisable[] _disabledComponents = Array.Empty<IPooledDisable>();
		private IPooledReset[] _resetsComponents = Array.Empty<IPooledReset>();
		private IPooledTeleportedLogic[] _teleportedLogicComponents = Array.Empty<IPooledTeleportedLogic>();
		private IPooledCustomTeleported _customTeleported;

		public GameObject GameObject
			=> gameObject;

		public void Enable()
		{
			_enabledComponents.Enable();
			gameObject.SetActive(true);
		}

		public void Disable()
		{
			_disabledComponents.Disable();
			gameObject.SetActive(false);
			DeadEvent?.Invoke(this);
		}

		public void Reset()
		{
			_resetsComponents.Reset();
		}

		public void TeleportTo(Vector3 position, Quaternion rotation)
		{
			_teleportedLogicComponents.DisableLogicBeforeTeleport();
			if (_customTeleported != null)
			{
				_customTeleported.TeleportTo(position, rotation);
			}
			else
			{
				transform.position = position;
				transform.rotation = rotation;
			}

			_teleportedLogicComponents.EnableLogicAfterTeleport();
		}

		public void CollectPooledComponents()
		{
			_enabledComponents = GetComponentsInChildren<IPooledEnable>();
			_disabledComponents = GetComponentsInChildren<IPooledDisable>();
			_resetsComponents = GetComponentsInChildren<IPooledReset>();
			_teleportedLogicComponents = GetComponentsInChildren<IPooledTeleportedLogic>();
			_customTeleported = GetComponent<IPooledCustomTeleported>();
		}

#if UNITY_EDITOR
		private void OnValidate()
		{
			var enabledComponents = GetComponents<IPooledEnable>();
			var disabledComponents = GetComponents<IPooledDisable>();
			var resetsComponents = GetComponents<IPooledReset>();
			var teleportedLogicComponents = GetComponents<IPooledTeleportedLogic>();
			var customTeleported = GetComponent<IPooledCustomTeleported>();

			if (!enabledComponents.GetHashCode().Equals(_enabledComponents.GetHashCode()))
				_enabledComponents = enabledComponents;
			if (!disabledComponents.GetHashCode().Equals(_disabledComponents.GetHashCode()))
				_disabledComponents = disabledComponents;
			if (!resetsComponents.GetHashCode().Equals(_resetsComponents.GetHashCode()))
				_resetsComponents = resetsComponents;
			if (!teleportedLogicComponents.GetHashCode().Equals(_teleportedLogicComponents.GetHashCode()))
				_teleportedLogicComponents = teleportedLogicComponents;
			if (!Equals(_customTeleported, customTeleported))
				_customTeleported = customTeleported;
		}
#endif
	}
}