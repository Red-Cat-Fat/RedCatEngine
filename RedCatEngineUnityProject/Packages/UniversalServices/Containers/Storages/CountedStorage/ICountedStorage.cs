using System.Collections.Generic;
using RedCatEngine.CommonServices.Containers.Storages.BaseStorage;

namespace RedCatEngine.CommonServices.Containers.Storages.CountedStorage
{
	public interface ICountedStorage<TItem> : IStorage<TItem>
	{
		int GetCount(TItem item);
		IEnumerable<TItem> GetItems();
		bool IsContain(TItem item, int count);
		void Add(TItem item, int count);
		void Remove(TItem item, int count);
	}
}