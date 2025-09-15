using System;
using System.Collections.Generic;
using System.Linq;

namespace RedCatEngine.CommonServices.Containers.Storages.BaseStorage
{
	public class BaseStorage<TItemContainer> : IStorage<TItemContainer>
	{
		protected readonly List<TItemContainer> _items = new();

		public event Action<TItemContainer, int> DeltaChangeElementEvent;
		public event Action<TItemContainer, int> FinalChangeElementEvent;
		public event Action ClearEvent;

		public IEnumerable<TItemContainer> GetElements()
			=> _items;

		public virtual bool IsContain(TItemContainer item)
			=> _items.Contains(item);

		public bool IsContainAny(IEnumerable<TItemContainer> itemsForCheck)
			=> _items.Any(itemsForCheck.Contains);

		public void Add(TItemContainer item)
		{
			_items.Add(item);
			DeltaChangeElementEvent?.Invoke(item, 1);
			FinalChangeElementEvent?.Invoke(item, 1);
		}

		public void Clear()
		{
			_items.Clear();
			ClearEvent?.Invoke();
		}

		public void Remove(TItemContainer item)
			=> _items.Remove(item);
	}
}