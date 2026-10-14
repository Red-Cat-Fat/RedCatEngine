using System;
using System.Collections.Generic;
using RedCatEngine.CommonServices.Services.Callbacks;
using RedCatEngine.CommonServices.Services.Times.TimeServices;
using RedCatEngine.DependencyInjection.Containers.Attributes;

namespace RedCatEngine.CommonServices.Services.DisposableCallbacks
{
	public class DisposableCallbackService : IDisposableCallbackService, IDisposable
	{
		// callbacks that are not timers, with a check whether they can still be invoked
		private readonly List<(IDisposable Callback, Func<bool> IsPending)> _callbacks = new();
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
			// copies: an invoke-on-dispose callback may create new callbacks while the lists are disposed
			foreach (var disposableTimerCallback in _timers.ToArray())
				disposableTimerCallback.Dispose();
			foreach (var (callback, _) in _callbacks.ToArray())
				callback.Dispose();

			_timers.Clear();
			_callbacks.Clear();
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
			Track(disposableCallback, () => disposableCallback.IsEnable);
			return disposableCallback;
		}

		public DisposableCallback MakeAlwaysInvokeCallback(Action callback)
		{
			var disposableCallback = new DisposableCallback(callback, true);
			Track(disposableCallback, () => disposableCallback.IsEnable);
			return disposableCallback;
		}

		public DisposableCallback<TResult> MakeCallback<TResult>(Action<TResult> callback)
		{
			var disposableCallback = new DisposableCallback<TResult>(callback);
			Track(disposableCallback, () => disposableCallback.IsEnable);
			return disposableCallback;
		}

		/// <summary>
		/// Keeps a callback so it is disposed together with the service, and drops callbacks that have already
		/// been invoked or disposed, so the list does not grow for the lifetime of the service.
		/// </summary>
		private void Track(IDisposable callback, Func<bool> isPending)
		{
			_callbacks.RemoveAll(entry => !entry.IsPending());
			_callbacks.Add((callback, isPending));
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