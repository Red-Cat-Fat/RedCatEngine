using RedCatEngine.Configs;
using RedCatEngine.DependencyInjection.Containers;
using RedCatEngine.Quests.Mechanics.Quests;

namespace RedCatEngine.Quests.Configs.Quests
{
	public abstract class QuestConfig : BaseConfig
	{
		public string Title;
		public string Description;

		public IQuest Make(IApplicationContainer applicationContainer) 
			=> DoMake(applicationContainer);

		protected abstract IQuest DoMake(IApplicationContainer applicationContainer);
	}
}