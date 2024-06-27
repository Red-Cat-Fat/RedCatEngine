using System;

namespace RedCatEngine.Quests.Utils.Time
{
	public class SimpleTimeService : ITimeService
	{
		public DateTime UtcNow => DateTime.UtcNow;
	}
}