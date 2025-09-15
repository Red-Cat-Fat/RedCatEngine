using System;
using System.Collections.Generic;
using RedCatEngine.CommonServices.Services.Callbacks;
using RedCatEngine.CommonServices.Services.Times.TimeServices;
using RedCatEngine.DependencyInjection.Containers.Attributes;

namespace RedCatEngine.CommonServices.Services.DisposableCallbacks
{
	public class DisposableCallbackService : IDisposableCallbackService, IDisposable
	{
		private readonly List<DisposableCallback> _disposable = new();
		private readonly List<DisposableTimerCallback> _timers = new();
		private readonly ITimeService _timeService;

		[Inject]
		public DisposableCallbackService(ITimeService timeService)
		{
			_timeService = timeService;
			_timeService.UpdateEvent += OnUpdate;
		}

		public void Dispose()
		{
			_timeService.UpdateEvent -= OnUpdate;
			foreach (var disposableTimerCallback in _timers)
				disposableTimerCallback.Dispose();
			foreach (var disposableCallback in _disposable)
				disposableCallback.Dispose();
		}

		public DisposableTimerCallback MakeTimerCallback(float time, Action callback)
		{
			var timerCallback = new DisposableTimerCallback(time, callback, false);
			_timers.Add(timerCallback);
			return timerCallback;
		}

		public DisposableTimerCallback MakeAlwaysInvokeTimerCallback(float time, Action callback)
		{
			var timerCallback = new DisposableTimerCallback(time, callback, true);
			_timers.Add(timerCallback);
			return timerCallback;
		}
		
		public DisposableCallback MakeCallback(Action callback)
		{
			var disposableCallback = new DisposableCallback(callback, false);
			_disposable.Add(disposableCallback);
			return disposableCallback;
		}

		public DisposableCallback MakeAlwaysInvokeCallback(Action callback)
		{
			var disposableCallback = new DisposableCallback(callback, true);
			_disposable.Add(disposableCallback);
			return disposableCallback;
		}

		private void OnUpdate(float deltaTime)
		{
			var currentFrameTimers = _timers.ToArray();
			foreach (var timerDisposableCallback in currentFrameTimers)
			{
				if (timerDisposableCallback == null)
					continue;
				timerDisposableCallback.OnUpdate(deltaTime);
				if (!timerDisposableCallback.IsEnable)
					timerDisposableCallback.Invoke();
			}

			_timers.RemoveAll(timer => timer == null || !timer.IsEnable);
		}
	}
}