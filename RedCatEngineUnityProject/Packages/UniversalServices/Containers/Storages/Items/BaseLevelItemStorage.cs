using System.Collections.Generic;
using RedCatEngine.CommonServices.Containers.Storages.CountedStorage;

namespace RedCatEngine.CommonServices.Containers.Storages.Items
{
	public class BaseLevelItemStorage<TItem> : BaseCountedStorage<TItem>, IItemStorage<TItem>
		where TItem : BaseItemConfig
	{
		private const int DefaultLevel = 1;

		private readonly Dictionary<TItem, int> _levels = new();

		public int GetLevel(TItem item)
			=> _levels.GetValueOrDefault(item, DefaultLevel);

		public void SetLevel(TItem item, int level)
		{
			if (_levels.TryGetValue(item, out _))
				_levels[item] = level;
			else
				_levels.Add(item, level);
		}
	}
}