using System;
using System.Collections.Generic;
using System.Linq;
using RedCatEngine.Conditions;
using RedCatEngine.Conditions.Base;
using RedCatEngine.Conditions.Variants;
using RedCatEngine.DependencyInjection.Containers.Interfaces;
using RedCatEngine.DependencyInjection.Containers.Interfaces.Application;
using RedCatEngine.Quests.Mechanics.Quests;
using SerializeReferenceEditor;
using UnityEngine;

namespace RedCatEngine.Quests.Configs.Quests
{
	[CreateAssetMenu(menuName = "Configs/Quests/QuestCollection/ConditionalQuestConfig", fileName = nameof(ConditionalQuestConfig))]
	public class ConditionalQuestConfig : QuestConfig, IQuestRedirected
	{
		public ConditionQuest[] Quests = Array.Empty<ConditionQuest>();
		
		[Serializable]
		public class ConditionQuest
		{
			[SR]
			[SerializeReference]
			public ICondition Condition = ForceCondition.True;

			public QuestConfig QuestConfig;
		}

		protected override IQuest DoMake(IApplicationContainer applicationContainer)
		{
			if (!applicationContainer.TryGetSingle<IConditionCheckerService>(out var conditionCheckerService))
			{
				Debug.LogError("Not found ConditionCheckerService");
				conditionCheckerService = new ConditionCheckerService(applicationContainer);
			}

			foreach (var quest in Quests)
			{
				if (conditionCheckerService.Check(quest.Condition))
					return quest.QuestConfig.Make(applicationContainer);
			}

			return null;
		}

		public IEnumerable<QuestConfig> GetAllQuestVariantForLoad()
			=> Quests.Select(conditionQuest => conditionQuest.QuestConfig);
	}
}