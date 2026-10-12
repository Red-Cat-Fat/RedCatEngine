namespace RedCatEngine.CommonServices.Services.Logs.Sinks
{
	public sealed class LogSinkControl : ILogSinkControl
	{
		private readonly UnityConsoleSink _consoleSink;
		private readonly FileLogSink _fileSink;

		public LogSinkControl(FileLogSink fileSink, UnityConsoleSink consoleSink)
		{
			_fileSink = fileSink;
			_consoleSink = consoleSink;
		}

		public bool IsFileEnabled
		{
			get => _fileSink != null && _fileSink.IsEnabled;
			set
			{
				if(_fileSink != null)
					_fileSink.IsEnabled = value;
			}
		}

		public bool IsUnityConsoleEnabled
		{
			get => _consoleSink != null && _consoleSink.IsEnabled;
			set
			{
				if(_consoleSink != null)
					_consoleSink.IsEnabled = value;
			}
		}

		public void Flush()
			=> _fileSink?.Flush();
	}
}
