using System;

namespace RedCatEngine.ApplicationRunner.Meta.PlayerModels.SaveTriggers
{
	public interface IPlayerModelSaveTrigger
	{
		event Action SaveEvent;
		void Save();
	}
}