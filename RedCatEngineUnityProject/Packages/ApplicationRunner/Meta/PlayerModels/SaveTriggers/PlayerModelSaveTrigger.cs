using System;
using JetBrains.Annotations;

namespace RedCatEngine.ApplicationRunner.Meta.PlayerModels.SaveTriggers
{
	[UsedImplicitly]
	public class PlayerModelSaveTrigger : IPlayerModelSaveTrigger
	{
		public event Action SaveEvent;

		public void Save()
			=> SaveEvent?.Invoke();
	}
}