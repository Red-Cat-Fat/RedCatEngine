using System.Collections.Generic;
using RedCatEngine.Configs;
using RedCatEngine.Quests.Configs.Quests;
using RedCatEngine.Quests.Mechanics.Factories;
using RedCatEngine.Quests.Mechanics.Quests;
using RedCatEngine.Quests.Mechanics.Quests.QuestDatas;

namespace RedCatEngine.Quests.Tests.SpecialSubClasses
{
	public class TestQuestFactory : IQuestFactory
	{
		private IQuest[] _returnQuest;
		private int _index = 0;

		public void SetReturnQuest(IQuest[] returnQuest)
		{
			_returnQuest = returnQuest;
			_index = 0;
		}

		public IQuest MakeFromConfig(ConfigID<QuestConfig> questConfig)
		{
			throw new System.NotImplementedException();
		}

		public IQuest MakeNewQuest(List<IQuest> currentActiveQuests)
		{
			throw new System.NotImplementedException();
		}

		public IQuest MakeNewQuest()
		{
			var result = _returnQuest[_index];
			_index++;
			return result;
		}

		public IQuest LoadFrom(IQuestData saveData)
		{
			foreach (var quest in _returnQuest)
				if (saveData.GetConfig() == quest.Config)
					return quest.LoadSave(saveData);
			return null;
		}

		public bool TryLoad(ConfigID<QuestConfig> questId, out QuestConfig questConfig)
		{
			throw new System.NotImplementedException();
		}
	}
}