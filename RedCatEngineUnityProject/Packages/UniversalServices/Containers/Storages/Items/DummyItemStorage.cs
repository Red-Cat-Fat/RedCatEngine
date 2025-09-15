using System;
using System.Collections.Generic;

namespace RedCatEngine.CommonServices.Containers.Storages.Items
{
	public class DummyItemStorage : IItemStorage<BaseItemConfig>
	{
		public static DummyItemStorage Empty
			=> new();

		public event Action<BaseItemConfig, int> DeltaChangeElementEvent;
		public event Action<BaseItemConfig, int> FinalChangeElementEvent;
		public event Action ClearEvent;

		private DummyItemStorage() { }

		public IEnumerable<BaseItemConfig> GetElements()
		{
			return ArraySegment<BaseItemConfig>.Empty;
		}

		public void Add(BaseItemConfig item) { }

		public bool IsContain(BaseItemConfig item)
		{
			return false;
		}

		public bool IsContainAny(IEnumerable<BaseItemConfig> itemsForCheck)
		{
			return false;
		}

		public void Clear()
		{
			ClearEvent?.Invoke();
		}

		public void Remove(BaseItemConfig item)
		{
			DeltaChangeElementEvent?.Invoke(item, 0);
			FinalChangeElementEvent?.Invoke(item, 0);
		}

		public int GetCount(BaseItemConfig item)
		{
			return 0;
		}

		public IEnumerable<BaseItemConfig> GetItems()
		{
			return Array.Empty<BaseItemConfig>();
		}

		public bool IsContain(BaseItemConfig item, int count)
		{
			return false;
		}

		public void Add(BaseItemConfig item, int count)
		{
			DeltaChangeElementEvent?.Invoke(item, 0);
			FinalChangeElementEvent?.Invoke(item, 0);
		}

		public void Remove(BaseItemConfig item, int count)
		{
			DeltaChangeElementEvent?.Invoke(item, 0);
			FinalChangeElementEvent?.Invoke(item, 0);
		}

		public int GetLevel(BaseItemConfig item)
		{
			return 1;
		}

		public void SetLevel(BaseItemConfig item, int level) { }
	}
}