using System;
using System.Collections.Generic;
using System.Linq;

namespace RedCatEngine.CommonServices.Containers.Storages.TypedStorage
{
	public class DynamicTypedStorage<TBaseTypeStorage> : ITypedStorage
	{
		private readonly List<TBaseTypeStorage> _items = new();

		public event Action<object> AddNewElementEvent;
		public event Action RemoveElementEvent;

		public event Action ClearEvent;

		public bool TryGets<TType>(out IEnumerable<TType> result)
		{
			result = _items.Where(item => item is TType).Cast<TType>();
			return result.Any();
		}

		public bool TryGet<TType>(out TType result)
		{
			var isContain = TryGets<TType>(out var collect);
			result = isContain ? collect.First() : default;
			return isContain;
		}

		public IEnumerable<TType> Gets<TType>()
		{
			return TryGets<TType>(out var result)
				? result
				: Array.Empty<TType>();
		}

		public void Add<TType>(TType item)
		{
			if (item is not TBaseTypeStorage baseType)
				return;

			_items.Add(baseType);
			AddNewElementEvent?.Invoke(item);
		}

		public bool IsContain<TType>()
			=> _items.Any(item => item is TType);

		public void Clear()
		{
			_items.Clear();
			ClearEvent?.Invoke();
		}

		public void Remove<TType>()
		{
			_items.RemoveAll(item => item is TType);
		}

		public void Remove(TBaseTypeStorage itemToRemove)
		{
			if (_items.Contains(itemToRemove))
				_items.Remove(itemToRemove);
			RemoveElementEvent?.Invoke();
		}
	}
}