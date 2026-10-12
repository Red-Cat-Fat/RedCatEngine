using System.Collections.Generic;

namespace RedCatEngine.CommonServices.Services.Logs.Catching
{
	public sealed class UnhandledLogBuffer : IUnhandledLogBuffer
	{
		private readonly object _lock = new();
		private readonly UnhandledLogRecord[] _records;
		private int _count;

		private int _head;

		public UnhandledLogBuffer(int capacity = 200)
			=> _records = new UnhandledLogRecord[capacity < 1 ? 1 : capacity];

		public int Count
		{
			get
			{
				lock(_lock)
					return _count;
			}
		}

		public IReadOnlyList<UnhandledLogRecord> GetRecent(int count)
		{
			lock(_lock)
			{
				if(count > _count)
					count = _count;

				var result = new List<UnhandledLogRecord>(count);
				for(var i = 0; i < count; i++)
				{
					var index = (_head - 1 - i + _records.Length * 2) % _records.Length;
					result.Add(_records[index]);
				}

				return result;
			}
		}

		public void Clear()
		{
			lock(_lock)
			{
				_head = 0;
				_count = 0;
			}
		}

		public void Push(in UnhandledLogRecord record)
		{
			lock(_lock)
			{
				_records[_head] = record;
				_head = (_head + 1) % _records.Length;

				if(_count < _records.Length)
					_count++;
			}
		}
	}
}
