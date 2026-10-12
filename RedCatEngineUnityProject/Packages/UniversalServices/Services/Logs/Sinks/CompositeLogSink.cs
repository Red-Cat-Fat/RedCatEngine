using System;

namespace RedCatEngine.CommonServices.Services.Logs.Sinks
{
	public sealed class CompositeLogSink : ILogSink
	{
		private readonly ILogSink[] _sinks;

		public CompositeLogSink(params ILogSink[] sinks)
			=> _sinks = sinks ?? Array.Empty<ILogSink>();

		public void Write(in LogEntry entry)
		{
			for(var i = 0; i < _sinks.Length; i++)
				_sinks[i].Write(in entry);
		}

		public void Flush()
		{
			for(var i = 0; i < _sinks.Length; i++)
				_sinks[i].Flush();
		}
	}
}
