using System;
using RedCatEngine.Quests.Utils.Time;

namespace RedCatEngine.Quests.Tests.SpecialSubClasses
{
	public class TestTimeService : ITimeService
	{
		public DateTime UtcNow { get; set; }
	}
}