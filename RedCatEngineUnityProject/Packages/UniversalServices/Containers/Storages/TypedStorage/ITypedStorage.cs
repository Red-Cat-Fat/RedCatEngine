using System;
using System.Collections.Generic;

namespace RedCatEngine.CommonServices.Containers.Storages.TypedStorage
{
	public interface ITypedStorage
	{
		event Action<object> AddNewElementEvent;
		event Action ClearEvent;
		bool TryGet<TType>(out TType result);
		bool TryGets<TType>(out IEnumerable<TType> result);
		IEnumerable<TType> Gets<TType>();

		public void Add<TType>(TType item);
		bool IsContain<TType>();
		void Clear();
		public void Remove<TType>();
	}
}