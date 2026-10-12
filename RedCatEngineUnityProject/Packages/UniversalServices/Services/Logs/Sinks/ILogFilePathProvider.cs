using System.Collections.Generic;

namespace RedCatEngine.CommonServices.Services.Logs.Sinks
{
	/// <summary>
	/// Доступ к пути файла логов без утечки абстракции логгера.
	/// Потребитель — сборщик баг-репортов, ему не нужен весь ILogService.
	/// </summary>
	public interface ILogFilePathProvider
	{
		string CurrentLogFilePath { get; }
		IReadOnlyList<string> SessionFilePaths { get; }
	}
}
