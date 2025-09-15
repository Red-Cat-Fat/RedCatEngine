using System;

namespace RedCatEngine.ApplicationRunner.Meta.PlayerModels
{
	public interface IPlayerModelContainer<TModelData>
	{
		event Action ReadyEvent;
		bool IsReady { get; }
		TModelData Model { get; }
		void Load();
		void Save();
#if CHEAT_ENABLE
		void ResetModel();
#endif
	}
}