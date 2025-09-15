using System;

namespace RedCatEngine.CommonServices.Services.Times.TimeServices
{
	public interface ITimeService
	{
		float FixedDeltaTime { get; }
		float DeltaTime { get; }
		float TimeScale { get; }
		float TotalTime { get; }
		event Action<float> UpdateEvent;
	}
}