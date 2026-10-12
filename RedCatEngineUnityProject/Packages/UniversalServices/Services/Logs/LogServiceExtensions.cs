using System.Diagnostics;

namespace RedCatEngine.CommonServices.Services.Logs
{
	/// <summary>
	/// Verbose/Info вынесены в extension-методы намеренно: [Conditional] запрещён на членах
	/// интерфейса (CS0582) и не удаляет вызов через интерфейсную ссылку. Здесь он работает
	/// и вырезает в релизном билде вызов ЦЕЛИКОМ, вместе с вычислением аргументов.
	///
	/// Несколько [Conditional] объединяются по ИЛИ: вызов остаётся, если определён хотя бы
	/// один из символов, поэтому отдельный дефайн для логов не нужен.
	///
	/// ВНИМАНИЕ: вырезается всё выражение вызова, включая receiver.
	/// GetLogger().Verbose(x) в релизе НЕ вызовет GetLogger().
	/// Слева от .Verbose( допустимы только поле или локальная переменная.
	///
	/// Строковую интерполяцию в аргументах использовать нельзя — она вычисляется до вызова
	/// и фильтром не отсекается. Только format + аргументы.
	/// </summary>
	public static class LogServiceExtensions
	{
		private const string EditorSymbol = "UNITY_EDITOR";
		private const string DevelopmentSymbol = "DEVELOPMENT_BUILD";
		private const string CheatSymbol = "CHEAT_ENABLED";

		#region Verbose

		[Conditional(EditorSymbol)]
		[Conditional(DevelopmentSymbol)]
		[Conditional(CheatSymbol)]
		public static void Verbose(this ILogService log, string message)
		{
			if(log != null && log.IsEnabled(LogLevel.Verbose))
				log.Write(LogLevel.Verbose, message);
		}

		[Conditional(EditorSymbol)]
		[Conditional(DevelopmentSymbol)]
		[Conditional(CheatSymbol)]
		public static void Verbose<T0>(this ILogService log, string format, T0 arg0)
		{
			if(log != null && log.IsEnabled(LogLevel.Verbose))
				log.Write(LogLevel.Verbose, string.Format(format, arg0));
		}

		[Conditional(EditorSymbol)]
		[Conditional(DevelopmentSymbol)]
		[Conditional(CheatSymbol)]
		public static void Verbose<T0, T1>(this ILogService log, string format, T0 arg0, T1 arg1)
		{
			if(log != null && log.IsEnabled(LogLevel.Verbose))
				log.Write(LogLevel.Verbose, string.Format(format, arg0, arg1));
		}

		[Conditional(EditorSymbol)]
		[Conditional(DevelopmentSymbol)]
		[Conditional(CheatSymbol)]
		public static void Verbose<T0, T1, T2>(this ILogService log, string format, T0 arg0, T1 arg1, T2 arg2)
		{
			if(log != null && log.IsEnabled(LogLevel.Verbose))
				log.Write(LogLevel.Verbose, string.Format(format, arg0, arg1, arg2));
		}

		[Conditional(EditorSymbol)]
		[Conditional(DevelopmentSymbol)]
		[Conditional(CheatSymbol)]
		public static void Verbose<T0, T1, T2, T3>(
				this ILogService log,
				string format,
				T0 arg0,
				T1 arg1,
				T2 arg2,
				T3 arg3
		)
		{
			if(log != null && log.IsEnabled(LogLevel.Verbose))
				log.Write(LogLevel.Verbose, string.Format(format, arg0, arg1, arg2, arg3));
		}

		[Conditional(EditorSymbol)]
		[Conditional(DevelopmentSymbol)]
		[Conditional(CheatSymbol)]
		public static void VerboseFormat(this ILogService log, string format, params object[] parameters)
		{
			if(log != null && log.IsEnabled(LogLevel.Verbose))
				log.Write(LogLevel.Verbose, string.Format(format, parameters));
		}

		#endregion

		#region Info

		[Conditional(EditorSymbol)]
		[Conditional(DevelopmentSymbol)]
		[Conditional(CheatSymbol)]
		public static void Info(this ILogService log, string message)
		{
			if(log != null && log.IsEnabled(LogLevel.Info))
				log.Write(LogLevel.Info, message);
		}

		[Conditional(EditorSymbol)]
		[Conditional(DevelopmentSymbol)]
		[Conditional(CheatSymbol)]
		public static void Info<T0>(this ILogService log, string format, T0 arg0)
		{
			if(log != null && log.IsEnabled(LogLevel.Info))
				log.Write(LogLevel.Info, string.Format(format, arg0));
		}

		[Conditional(EditorSymbol)]
		[Conditional(DevelopmentSymbol)]
		[Conditional(CheatSymbol)]
		public static void Info<T0, T1>(this ILogService log, string format, T0 arg0, T1 arg1)
		{
			if(log != null && log.IsEnabled(LogLevel.Info))
				log.Write(LogLevel.Info, string.Format(format, arg0, arg1));
		}

		[Conditional(EditorSymbol)]
		[Conditional(DevelopmentSymbol)]
		[Conditional(CheatSymbol)]
		public static void Info<T0, T1, T2>(this ILogService log, string format, T0 arg0, T1 arg1, T2 arg2)
		{
			if(log != null && log.IsEnabled(LogLevel.Info))
				log.Write(LogLevel.Info, string.Format(format, arg0, arg1, arg2));
		}

		[Conditional(EditorSymbol)]
		[Conditional(DevelopmentSymbol)]
		[Conditional(CheatSymbol)]
		public static void Info<T0, T1, T2, T3>(
				this ILogService log,
				string format,
				T0 arg0,
				T1 arg1,
				T2 arg2,
				T3 arg3
		)
		{
			if(log != null && log.IsEnabled(LogLevel.Info))
				log.Write(LogLevel.Info, string.Format(format, arg0, arg1, arg2, arg3));
		}

		[Conditional(EditorSymbol)]
		[Conditional(DevelopmentSymbol)]
		[Conditional(CheatSymbol)]
		public static void InfoFormat(this ILogService log, string format, params object[] parameters)
		{
			if(log != null && log.IsEnabled(LogLevel.Info))
				log.Write(LogLevel.Info, string.Format(format, parameters));
		}

		#endregion
	}
}
