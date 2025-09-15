using System;
using System.Collections.Generic;
using System.Linq;

namespace RedCatEngine.Pools.Objects
{
	public class PooledObjectTypedEnumerator<TReturnEnumeratorType> : IEnumerator<TReturnEnumeratorType>
	{
		private readonly TReturnEnumeratorType[] _collection;
		private int _index = -1;

		public PooledObjectTypedEnumerator(IEnumerable<TReturnEnumeratorType> collection)
		{
			_collection = collection.ToArray();
		}

		private TReturnEnumeratorType TypedCurrent
		{
			get
			{
				try
				{
					return _collection[_index];
				}
				catch (IndexOutOfRangeException)
				{
					throw new InvalidOperationException();
				}
			}
		}

		TReturnEnumeratorType IEnumerator<TReturnEnumeratorType>.Current
			=> TypedCurrent;

		public object Current
			=> TypedCurrent;

		public bool MoveNext()
		{
			_index++;
			return _index < _collection.Length;
		}

		public void Reset()
		{
			_index = -1;
		}

		public void Dispose()
		{
		}
	}
}