using System;
using RedCatEngine.Configs;
using RedCatEngine.Quests.Configs.Quests;
using RedCatEngine.Quests.Mechanics.Quests.QuestDatas;

namespace RedCatEngine.Quests.Mechanics.Quests
{
	public abstract class BaseSavedQuest<TQuestData> : BaseQuest, IQuest where TQuestData : BaseQuestData, new()
	{
		protected TQuestData Data = new();

		protected BaseSavedQuest(ConfigID<QuestConfig> config) : base(config)
		{
		}

		public sealed override IQuest LoadSave(IQuestData data)
			=> DoLoadSave(data as TQuestData);

		public sealed override IQuestData GetData()
			=> DoSaveData();

		private IQuest DoLoadSave(TQuestData saveData)
		{
			return saveData == null
				? this //todo: make validation data
				: DoLoadData(saveData);
		}

		private TQuestData DoSaveData()
		{
			var saveData = new TQuestData
			{
				Config = Config,
				CreateTime = Data.CreateTime,
				QuestState = Data.QuestState
			};
			return DoSaveData(saveData);
		}

		protected abstract IQuest DoLoadData(TQuestData saveData);
		protected abstract TQuestData DoSaveData(TQuestData saveData);

		protected override void SendComplete()
		{
			Data.QuestState = QuestState.Complete;
			base.SendComplete();
		}

		protected override void DoStart(DateTime dateTime)
		{
			Data.CreateTime = dateTime;
		}
	}
}