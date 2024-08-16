using System;

namespace RedCatEngine.Quests.Utils
{
	[Serializable]
	public class SerializedDateTime
	{
		public static SerializedDateTime Now
			=> (SerializedDateTime)DateTime.Now;
		public int Year;
		public int Month;
		public int Day;

		public int Hour;
		public int Minute;
		public int Seconds;

		private DateTime GetDateTime()
		{
			return new DateTime(
				year: Year,
				month: Month,
				day: Day,
				hour: Hour,
				minute: Minute,
				second: Seconds);
		}

		private SerializedDateTime FromDateTime(DateTime dateTime)
		{
			Year = dateTime.Year;
			Month = dateTime.Month;
			Day = dateTime.Day;
			Hour = dateTime.Hour;
			Minute = dateTime.Minute;
			Seconds = dateTime.Second;
			return this;
		}

		public static explicit operator DateTime(SerializedDateTime serializedDateTime)
			=> serializedDateTime.GetDateTime();

		public static explicit operator SerializedDateTime(DateTime serializedDateTime)
			=> new SerializedDateTime().FromDateTime(serializedDateTime);
	}
}