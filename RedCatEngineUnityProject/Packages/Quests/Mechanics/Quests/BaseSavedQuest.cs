using RedCatEngine.Configs;
using RedCatEngine.Quests.Configs.Quests;
using RedCatEngine.Quests.Mechanics.Quests.QuestDatas;
using RedCatEngine.Rewards.Base;

namespace RedCatEngine.Quests.Mechanics.Quests
{
	public abstract class BaseSavedQuest<TQuestData> : BaseQuest where TQuestData : BaseQuestData, new()
	{
		protected BaseSavedQuest(ConfigID<QuestConfig> config, IReward reward) : base(config, reward)
		{
		}

		public sealed override IQuest LoadSave(IQuestData data)
		{
			if (data == null)
				return this;

			SetStartTime(data.GetCreateTime());
			SetQuestState(data.GetQuestState());

			var result = DoLoadData(data as TQuestData);
			
			CheckComplete();
			return result;
		}

		public sealed override IQuestData GetData()
			=> DoSaveData();

		private TQuestData DoSaveData()
		{
			var saveData = new TQuestData();
			saveData.ConstructSerialization(
				Config,
				StartQuestTime,
				QuestState);
			return DoSaveData(saveData);
		}

		protected abstract IQuest DoLoadData(TQuestData saveData);
		protected abstract TQuestData DoSaveData(TQuestData saveData);
	}
}