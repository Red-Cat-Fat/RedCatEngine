using System;
using RedCatEngine.CommonServices.Services.Callbacks;

namespace RedCatEngine.CommonServices.Services.DisposableCallbacks
{
	public interface IDisposableCallbackService
	{
		DisposableTimerCallback MakeTimerCallback(float time, Action callback);
		DisposableTimerCallback MakeAlwaysInvokeTimerCallback(float time, Action callback);
		DisposableCallback MakeCallback(Action callback);
		DisposableCallback MakeAlwaysInvokeCallback(Action callback);
	}
}