using System;
using System.Collections.Generic;
using System.Linq;

namespace RedCatEngine.CommonServices.Containers.Storages.CountedStorage
{
	public class BaseCountedStorage<TItem> : ICountedStorage<TItem>
	{
		private readonly Dictionary<TItem, int> _container = new();
		public event Action<TItem, int> DeltaChangeElementEvent;
		public event Action<TItem, int> FinalChangeElementEvent;
		public event Action ClearEvent;

		public IEnumerable<TItem> GetElements()
			=> _container.Keys.Where(item => IsContain(item));

		public void Add(TItem item)
			=> Add(item, 1);

		public void Add(TItem item, int addCount)
		{
			if (_container.TryGetValue(item, out var currentCount))
				_container[item] = currentCount + addCount;
			else
				_container.Add(item, addCount);
			DeltaChangeElementEvent?.Invoke(item, addCount);
			FinalChangeElementEvent?.Invoke(item, GetCount(item));
		}

		public bool IsContain(TItem item)
			=> _container.TryGetValue(item, out var count) && count > 0;

		public bool IsContain(TItem item, int count)
			=> IsContain(item) && _container[item] >= count;

		public bool IsContainAny(IEnumerable<TItem> itemsForCheck)
			=> itemsForCheck.Any(IsContain);

		public void Clear()
		{
			_container.Clear();
			ClearEvent?.Invoke();
		}

		public void Remove(TItem item)
			=> Remove(item, 1);

		public void Remove(TItem item, int count)
		{
			if (!_container.TryGetValue(item, out var currentCount))
				return;

			_container[item] = Math.Max(currentCount - count, 0);
			DeltaChangeElementEvent?.Invoke(item, -Math.Min(currentCount, -count));
			FinalChangeElementEvent?.Invoke(item, GetCount(item));
		}

		public int GetCount(TItem item)
			=> _container.GetValueOrDefault(item, 0);

		public IEnumerable<TItem> GetItems()
		{
			return _container.Keys;
		}
	}
}