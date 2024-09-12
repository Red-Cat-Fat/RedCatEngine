using System.Collections.Generic;
using System.Linq;
using RedCatEngine.DependencyInjection.Containers.Interfaces;
using RedCatEngine.DependencyInjection.Containers.Interfaces.Application;
using RedCatEngine.Quests.Mechanics.Quests;
using UnityEngine;

namespace RedCatEngine.Quests.Configs.Quests
{
	public abstract class GroupQuestConfig<TQuestType> : QuestConfig, IQuestRedirected where TQuestType : QuestConfig
	{
		public TQuestType[] Quests;
		protected override IQuest DoMake(IApplicationContainer applicationContainer)
			=> Quests.Length == 0 ? null : Quests[Random.Range(0, Quests.Length)].Make(applicationContainer);

		public IEnumerable<QuestConfig> GetAllQuestVariantForLoad()
			=> Quests.Select(quest=>(QuestConfig) quest);
	}
}