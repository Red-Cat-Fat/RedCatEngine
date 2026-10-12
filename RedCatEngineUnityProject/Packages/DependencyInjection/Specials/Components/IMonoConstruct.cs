namespace RedCatEngine.DependencyInjection.Specials.Components
{
	public interface IMonoConstruct
	{
		bool IsInitialized { get; }
		public void FinishInitialize();
	}
}