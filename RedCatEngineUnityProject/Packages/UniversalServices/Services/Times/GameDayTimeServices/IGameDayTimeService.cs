using System;

namespace RedCatEngine.CommonServices.Services.Times.GameDayTimeServices
{
	public interface IGameDayTimeService
	{
		DateTime CurrentTime { get; }
		event Action SunriseEvent;
		event Action SunsetEvent;
		event Action HourChangeEvent;

		float CalculateSunAngle();
#if CHEAT_ENABLED
		float TimeMultiplier { get; set; }
#endif
	}
}