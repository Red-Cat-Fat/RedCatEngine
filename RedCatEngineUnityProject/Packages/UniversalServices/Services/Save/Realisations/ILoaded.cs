namespace RedCatEngine.CommonServices.Services.Save.Realisations
{
	public interface ILoaded
	{
		string SaveKey { get; }
		void Load(string serializedSaveData);
	}
}