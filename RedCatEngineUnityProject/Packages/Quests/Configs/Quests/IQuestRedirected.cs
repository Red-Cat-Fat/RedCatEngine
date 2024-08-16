using System.Collections.Generic;

namespace RedCatEngine.Quests.Configs.Quests
{
	public interface IQuestRedirected
	{
		public IEnumerable<QuestConfig> GetAllQuestVariantForLoad();
	}
}