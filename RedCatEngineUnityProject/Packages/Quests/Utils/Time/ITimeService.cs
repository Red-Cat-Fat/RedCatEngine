using System;

namespace RedCatEngine.Quests.Utils.Time
{
	public interface ITimeService 
		//todo: move ITimeService to special Utils package
	{
		DateTime UtcNow { get; }
	}
}