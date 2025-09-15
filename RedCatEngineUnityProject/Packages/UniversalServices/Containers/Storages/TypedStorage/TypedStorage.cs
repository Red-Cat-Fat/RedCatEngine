using System;
using System.Collections.Generic;
using System.Linq;

namespace RedCatEngine.CommonServices.Containers.Storages.TypedStorage
{
	public class TypedStorage : ITypedStorage
	{
		private readonly Dictionary<Type, List<object>> _items = new();

		public event Action<object> AddNewElementEvent;

		public event Action ClearEvent;

		protected virtual void AddToDictionary<TType>(TType item)
		{
			if (!_items.TryGetValue(typeof(TType), out var elementsList))
			{
				elementsList = new List<object>();
				_items.Add(typeof(TType), elementsList);
			}

			elementsList.Add(item);
			AddNewElementEvent?.Invoke(item);
		}

		public bool TryGets<TType>(out IEnumerable<TType> result)
		{
			var isContain = _items.TryGetValue(typeof(TType), out var collect);
			result = isContain ? collect.Cast<TType>() : Array.Empty<TType>();
			return isContain;
		}
		
		public bool TryGet<TType>(out TType result)
		{
			var isContain = _items.TryGetValue(typeof(TType), out var collect) && collect.Any();
			if(isContain)
				result = (TType)collect.First();
			else
				result = default;
			return isContain;
		}

		public IEnumerable<TType> Gets<TType>()
		{
			return _items.TryGetValue(typeof(TType), out var result)
				? result.Cast<TType>()
				: Array.Empty<TType>();
		}

		public void Add<TType>(TType item)
			=> AddToDictionary(item);

		public bool IsContain<TType>()
			=> _items.TryGetValue(typeof(TType), out var items) && items.Any();

		public void Clear()
		{
			_items.Clear();
			ClearEvent?.Invoke();
		}

		public void Remove<TType>()
		{
			if (_items.TryGetValue(typeof(TType), out var items))
				items.Clear();
		}
	}
}