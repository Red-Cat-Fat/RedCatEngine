using RedCatEngine.CommonServices.Services.Logs.Catching;
using RedCatEngine.CommonServices.Services.Logs.Filtering;
using RedCatEngine.CommonServices.Services.Logs.Sinks;

namespace RedCatEngine.CommonServices.Services.Logs
{
	/// <summary>
	/// Статическая точка доступа к логированию. Осознанное отступление от «никаких new Service()»:
	/// логгер нужен раньше DI — в инициализаторах полей, в OnValidate, в статических утилитах,
	/// в коде, который падает до того, как контейнер собран.
	///
	/// До инициализации отдаёт NullLogService, поэтому Root никогда не null.
	/// </summary>
	public static class LogHub
	{
		public static ILogService Root { get; private set; } = NullLogService.Instance;
		public static ILogLevelFilter Filter { get; private set; } = AlwaysAllowFilter.Instance;
		public static ILogFilePathProvider FilePath { get; private set; }
		public static IUnhandledLogBuffer Unhandled { get; private set; }
		public static LogRouter Router { get; private set; }

		public static bool IsInitialized { get; private set; }

		/// <summary>
		/// Инкрементируется на каждой инициализации. Кэшируйте логгер вместе с этим номером:
		/// при выключенном Domain Reload статика переживает выход из Play Mode и иначе
		/// продолжила бы указывать на уже закрытый файловый синк.
		/// </summary>
		public static int Generation { get; private set; }

		public static void Initialize(
				ILogService root,
				ILogLevelFilter filter,
				LogRouter router,
				ILogFilePathProvider filePath,
				IUnhandledLogBuffer unhandled
		)
		{
			Root = root ?? NullLogService.Instance;
			Filter = filter ?? AlwaysAllowFilter.Instance;
			Router = router;
			FilePath = filePath;
			Unhandled = unhandled;
			IsInitialized = true;
			Generation++;
		}

		public static void Reset()
		{
			Root = NullLogService.Instance;
			Filter = AlwaysAllowFilter.Instance;
			Router = null;
			FilePath = null;
			Unhandled = null;
			IsInitialized = false;
		}

		public static ILogService CreateTag(string tag)
			=> Root.CreateTag(tag);

		public static ILogService CreateTag<TType>()
			=> Root.CreateTag<TType>();
	}
}
