namespace RedCatEngine.CommonServices.Services.Save.Realisations
{
	public interface ISaved : ILoaded
	{
		string GetSerializedSaveData();
	}
}