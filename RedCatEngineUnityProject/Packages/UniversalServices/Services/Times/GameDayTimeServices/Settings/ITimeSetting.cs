namespace RedCatEngine.CommonServices.Services.Times.GameDayTimeServices.Settings
{
	public interface ITimeSetting
	{
		public float TimeMultiplier { get; }
		public float StartHour { get; }
		public float SunriseHour { get; }
		public float SunsetHour { get; }
	}
}