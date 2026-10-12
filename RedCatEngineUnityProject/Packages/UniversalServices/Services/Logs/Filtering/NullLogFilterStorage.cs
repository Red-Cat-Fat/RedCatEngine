namespace RedCatEngine.CommonServices.Services.Logs.Filtering
{
	/// <summary>Ничего не сохраняет. Используется в релизных билдах.</summary>
	public sealed class NullLogFilterStorage : ILogFilterStorage
	{
		public bool TryLoad(out string globalLevel, out string overrides)
		{
			globalLevel = null;
			overrides = null;
			return false;
		}

		public void Save(string globalLevel, string overrides)
		{
		}

		public void Clear()
		{
		}
	}
}
