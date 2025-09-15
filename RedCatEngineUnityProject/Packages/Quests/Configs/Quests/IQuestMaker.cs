using RedCatEngine.DependencyInjection.Containers.Interfaces.Application;
using RedCatEngine.Quests.Mechanics.Quests;

namespace RedCatEngine.Quests.Configs.Quests
{
	public interface IQuestMaker
	{
		IQuest Make(IApplicationContainer applicationContainer);
	}
}