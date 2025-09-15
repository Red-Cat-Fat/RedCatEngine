using RedCatEngine.Configs;
using RedCatEngine.Quests.Configs.Quests;

namespace RedCatEngine.Quests.Mechanics.Quests
{
	public interface IQuestDescription
	{
		ConfigID<QuestConfig> Config { get; }
		QuestState QuestState { get; }

		double GetProgress();
		string GetProcessProgressText();
		string GetLocalizedName();
		string GetLocalizedDescription();
	}
}