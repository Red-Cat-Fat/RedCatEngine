namespace RedCatEngine.DependencyInjection.Specials.Diagnostics
{
	public class NullContainerDiagnostics : IContainerDiagnostics
	{
		public static readonly NullContainerDiagnostics Instance = new();

		private NullContainerDiagnostics()
		{
		}

		public void Trace(string format, params object[] args)
		{
		}

		public void Error(string format, params object[] args)
		{
		}
	}
}
