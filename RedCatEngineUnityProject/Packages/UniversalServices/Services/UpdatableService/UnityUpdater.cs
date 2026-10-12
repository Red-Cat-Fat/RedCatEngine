using System;
using UnityEngine;

namespace RedCatEngine.CommonServices.Services.UpdatableService
{
	public class UnityUpdater : MonoBehaviour, IUpdateTrigger
	{
		public event Action UpdateEvent;
		public event Action FixedUpdateEvent;
		public event Action LateUpdateEvent;

		public void Update()
			=> UpdateEvent?.Invoke();

		public void FixedUpdate()
			=> FixedUpdateEvent?.Invoke();

		public void LateUpdate()
			=> LateUpdateEvent?.Invoke();
	}
}