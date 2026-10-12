namespace RedCatEngine.CommonServices.Services.Logs.Filtering
{
	/// <summary>
	/// Персист настроек фильтра. Значения — строки, чтобы не тащить сериализацию в пакет.
	/// Формат оверрайдов: "Npc=Verbose;Network:Voice=Off;Ai=Warning".
	/// </summary>
	public interface ILogFilterStorage
	{
		bool TryLoad(out string globalLevel, out string overrides);
		void Save(string globalLevel, string overrides);
		void Clear();
	}
}
