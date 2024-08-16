using RedCatEngine.Configs;
using RedCatEngine.DependencyInjection.Containers.Interfaces;
using RedCatEngine.Quests.Mechanics.Quests;
using RedCatEngine.Quests.Mechanics.Quests.QuestDatas;

namespace RedCatEngine.Quests.Configs.Quests
{
	public abstract class QuestConfig : BaseConfig
	{
		public IQuest Make(IApplicationContainer applicationContainer) 
			=> DoMake(applicationContainer);

		protected abstract IQuest DoMake(IApplicationContainer applicationContainer);

		public IQuest Make(IApplicationContainer applicationContainer, IQuestData saveData)
		{
			var makeResult = DoMake(applicationContainer);
			return makeResult.LoadSave(saveData);
		}
	}
}