using System;
using System.Collections.Generic;
using UnityEngine;

namespace RedCatEngine.CommonServices.Services.Times.TimeServices
{
	public class UnityTimeService : MonoBehaviour, ITimeService, IPausedTimeService
	{
		private readonly List<object> _pauseHolder = new();

		public float FixedDeltaTime
			=> Time.fixedDeltaTime;

		public float DeltaTime
			=> Time.deltaTime;

		public float TimeScale
			=> Time.timeScale;

		public float TotalTime 
			=> Time.time;

		private void Update()
			=> UpdateEvent?.Invoke(DeltaTime);

		public void Pause(object source)
		{
			if (_pauseHolder.Contains(source))
				return;
			_pauseHolder.Add(source);
			UpdatePauseState();
		}

		public void UnPause(object source)
		{
			if (!_pauseHolder.Contains(source))
				return;
			_pauseHolder.Remove(source);
			UpdatePauseState();
		}

		public event Action<float> UpdateEvent;

		private void UpdatePauseState()
		{
			Time.timeScale = _pauseHolder.Count > 0
				? 0.001f
				: 1f; //todo: merge with IInputService
		}
	}
}