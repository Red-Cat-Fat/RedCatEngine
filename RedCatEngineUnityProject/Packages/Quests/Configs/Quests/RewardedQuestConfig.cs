using RedCatEngine.Rewards.Base;
using SerializeReferenceEditor;
using UnityEngine;

namespace RedCatEngine.Quests.Configs.Quests
{
	public abstract class RewardedQuestConfig : QuestConfig
	{
		[SR]
		[SerializeReference]
		public IReward Reward;
	}
}