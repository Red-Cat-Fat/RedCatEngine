using RedCatEngine.Configs;
using UnityEngine;

namespace RedCatEngine.Quests.Configs.Quests
{
	[CreateAssetMenu(
		fileName = nameof(AllQuestLinksConfig),
		menuName = "Configs/Quests/Quest Systems/AllQuestCollection",
		order = 0)]
	public class AllQuestLinksConfig : BaseAllConfigContainerConfig<QuestConfig>
	{
	}
}