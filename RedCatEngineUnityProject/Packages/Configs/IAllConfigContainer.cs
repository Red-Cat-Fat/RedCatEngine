namespace RedCatEngine.Configs
{
	public interface IAllConfigContainer<TConfig> where TConfig : BaseConfig
	{
		public TConfig[] AllConfigs { get; }
		bool TryGetById(uint configId, out TConfig tag);
	}
}