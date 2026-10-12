namespace RedCatEngine.CommonServices.Services.Logs.Sinks
{
	/// <summary>Управление синками из консоли, без доступа к их реализации.</summary>
	public interface ILogSinkControl
	{
		bool IsFileEnabled { get; set; }
		bool IsUnityConsoleEnabled { get; set; }

		void Flush();
	}
}
