namespace RedCatEngine.CommonServices.Services.Logs
{
	/// <summary>
	/// Боевой логгер: отдаёт сообщения роутеру, который раскладывает их по синкам
	/// (Unity-консоль и файл). Уровень резолвится фильтром роутера.
	/// </summary>
	public sealed class RoutedLogService : BaseLogService
	{
		private readonly LogRouter _router;

		public RoutedLogService(LogRouter router)
				: this(null, router)
		{
		}

		private RoutedLogService(string tag, LogRouter router)
				: base(tag, router.Filter)
			=> _router = router;

		protected override ILogService DoCreateInstance(string tag)
			=> new RoutedLogService(tag, _router);

		protected override void DoWrite(LogLevel level, string message)
			=> _router.Write(level, Tag, message);
	}
}
