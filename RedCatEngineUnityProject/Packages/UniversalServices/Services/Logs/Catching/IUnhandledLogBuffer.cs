using System.Collections.Generic;

namespace RedCatEngine.CommonServices.Services.Logs.Catching
{
	/// <summary>
	/// Кольцевой буфер последних ошибок и исключений со стектрейсами.
	/// Пишется с любого потока, читается с главного (консоль, баг-репорт).
	/// </summary>
	public interface IUnhandledLogBuffer
	{
		int Count { get; }
		IReadOnlyList<UnhandledLogRecord> GetRecent(int count);
		void Clear();
	}
}
