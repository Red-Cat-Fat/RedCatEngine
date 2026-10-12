using System;
using UnityEngine;

namespace RedCatEngine.CommonServices.Services.Logs
{
	/// <summary>
	/// Теговый логгер. Теги иерархические, разделитель — ':'.
	/// Verbose/Info пишутся через extension-методы <see cref="LogServiceExtensions"/>,
	/// они помечены [Conditional] и вырезаются в релизных билдах вместе с аргументами.
	/// </summary>
	public interface ILogService
	{
		string Tag { get; }

		/// <summary>Короткий путь для горячих мест, чтобы не гонять enum.</summary>
		bool IsVerbose { get; }

		/// <summary>
		/// Структурный тег — попадает в реестр и виден в log_tags / редакторном окне.
		/// </summary>
		ILogService CreateTag(string tag);

		ILogService CreateTag<TType>();

		/// <summary>
		/// Тег конкретного экземпляра (имя объекта, netId). В реестр НЕ попадает,
		/// но участвует в иерархическом резолве уровня как обычный суффикс.
		/// </summary>
		ILogService CreateInstanceTag(string instanceName);

		bool IsEnabled(LogLevel level);

		void Write(LogLevel level, string message);
		void WriteException(Exception exception, string context);

		void LogWarning(string log);
		void LogError(string log);
		void LogWarningFormat(string log, params object[] parameters);
		void LogErrorFormat(string log, params object[] parameters);

		public ILogService CreateTag(GameObject gameObject)
			=> CreateInstanceTag(gameObject.name);
	}
}
