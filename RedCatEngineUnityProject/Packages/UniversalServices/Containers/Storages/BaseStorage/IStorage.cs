using System;
using System.Collections.Generic;
using RedCatEngine.CommonServices.Containers.Components;

namespace RedCatEngine.CommonServices.Containers.Storages.BaseStorage
{
	public interface IStorage<TItem> : IRedComponent
	{
		event Action<TItem, int> DeltaChangeElementEvent;
		event Action<TItem, int> FinalChangeElementEvent;
		event Action ClearEvent;
		IEnumerable<TItem> GetElements();
		void Add(TItem item);
		bool IsContain(TItem item);
		bool IsContainAny(IEnumerable<TItem> itemsForCheck);
		void Clear();
		void Remove(TItem item);
	}
}