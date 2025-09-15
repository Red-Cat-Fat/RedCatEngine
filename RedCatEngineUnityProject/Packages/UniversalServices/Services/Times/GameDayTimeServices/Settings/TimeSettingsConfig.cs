using RedCatEngine.Configs;
using UnityEngine;

namespace RedCatEngine.CommonServices.Services.Times.GameDayTimeServices.Settings
{
	[CreateAssetMenu(menuName = "Configs/Common/Time Settings Config")]
	public class TimeSettingsConfig : BaseConfig, ITimeSetting
	{
		[SerializeField] private float _timeMultiplier = 2000;

		[SerializeField] private float _startHour = 12;

		[SerializeField] private float _sunriseHour = 6;

		[SerializeField] private float _sunsetHour = 18;
		public float TimeMultiplier
			=> _timeMultiplier;
		public float StartHour
			=> _startHour;
		public float SunriseHour
			=> _sunriseHour;
		public float SunsetHour
			=> _sunsetHour;
	}
}