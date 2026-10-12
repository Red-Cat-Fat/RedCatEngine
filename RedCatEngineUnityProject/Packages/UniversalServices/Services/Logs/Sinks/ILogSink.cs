namespace RedCatEngine.CommonServices.Services.Logs.Sinks
{
	public interface ILogSink
	{
		void Write(in LogEntry entry);
		void Flush();
	}
}
