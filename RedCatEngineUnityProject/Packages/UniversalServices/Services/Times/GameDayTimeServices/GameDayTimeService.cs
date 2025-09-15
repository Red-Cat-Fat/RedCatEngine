using System;
using RedCatEngine.CommonServices.Containers.Observables;
using RedCatEngine.CommonServices.Services.Times.GameDayTimeServices.Settings;
using RedCatEngine.CommonServices.Services.Times.TimeServices;
using RedCatEngine.DependencyInjection.Containers.Attributes;
using UnityEngine;

namespace RedCatEngine.CommonServices.Services.Times.GameDayTimeServices
{
	public class GameDayTimeService : IGameDayTimeService, IDisposable
	{
		private readonly ITimeService _timeService;
		private readonly ITimeSetting _settings;
		private readonly TimeSpan _sunriseTime;
		private readonly TimeSpan _sunsetTime;
		private DateTime _currentTime;

		public DateTime CurrentTime
			=> _currentTime;

		public event Action SunriseEvent;
		public event Action SunsetEvent;
		public event Action HourChangeEvent;

		private readonly Observable<bool> _isDayTime;
		private readonly Observable<int> _currentHour;

		[Inject]
		public GameDayTimeService(ITimeService timeService, ITimeSetting settings)
		{
			_timeService = timeService;
			_settings = settings;
			_currentTime = DateTime.Now.Date + TimeSpan.FromHours(settings.StartHour);
			_sunriseTime = TimeSpan.FromHours(settings.SunriseHour);
			_sunsetTime = TimeSpan.FromHours(settings.SunsetHour);

			_isDayTime = new Observable<bool>(IsDayTime());
			_currentHour = new Observable<int>(_currentTime.Hour);

			_timeService.UpdateEvent += UpdateTime;
			_isDayTime.ValueChangeEvent += OnDayTimeChanged;
			_currentHour.ValueChangeEvent += OnHourChange;
		}

		private void OnHourChange(int hour)
			=> HourChangeEvent?.Invoke();

		private void OnDayTimeChanged(bool day)
			=> (day ? SunriseEvent : SunsetEvent)?.Invoke();

		private void UpdateTime(float deltaTime)
		{
			var addSeconds = deltaTime * _settings.TimeMultiplier;

#if CHEAT_ENABLED
			addSeconds *= TimeMultiplier;
#endif
			_currentTime = _currentTime.AddSeconds(addSeconds);
			_isDayTime.Value = IsDayTime();
			_currentHour.Value = _currentTime.Hour;
		}

		public float CalculateSunAngle()
		{
			var isDay = IsDayTime();
			float startDegree = isDay ? 0 : 180;
			var start = isDay ? _sunriseTime : _sunsetTime;
			var end = isDay ? _sunsetTime : _sunriseTime;

			var totalTime = CalculateDifference(start, end);
			var elapsedTime = CalculateDifference(start, _currentTime.TimeOfDay);

			var percentage = elapsedTime.TotalMinutes / totalTime.TotalMinutes;
			return Mathf.Lerp(
				startDegree,
				startDegree + 180,
				(float)percentage);
		}
#if CHEAT_ENABLED
		public float TimeMultiplier { get; set; } = 1f;
#endif

		// This method checks whether the current game time falls within the daytime period.
		// It returns true if the current time of day is later than sunriseTime and earlier than sunsetTime,
		// representing daytime. Otherwise, it returns false, indicating it is nighttime.
		private bool IsDayTime()
			=> _currentTime.TimeOfDay > _sunriseTime && _currentTime.TimeOfDay < _sunsetTime;

		// This method calculates the difference between two TimeSpan objects ("from" and "to").
		// If the calculated difference is negative, this indicates that the "from" time is ahead of the "to" time.
		// In such cases, 24 hours (representing a full day) is added to the negative difference to calculate the actual
		// time difference taking into account the next day.    
		private TimeSpan CalculateDifference(TimeSpan from, TimeSpan to)
		{
			var difference = to - from;
			return difference.TotalHours < 0 ? difference + TimeSpan.FromHours(24) : difference;
		}

		public void Dispose()
		{
			_timeService.UpdateEvent -= UpdateTime;
			_isDayTime.ValueChangeEvent -= OnDayTimeChanged;
			_currentHour.ValueChangeEvent -= OnHourChange;
		}
	}
}