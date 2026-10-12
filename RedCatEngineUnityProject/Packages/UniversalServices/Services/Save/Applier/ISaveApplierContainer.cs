using RedCatEngine.CommonServices.Services.Save.Data;
using RedCatEngine.CommonServices.Services.Save.Realisations;
using RedCatEngine.DependencyInjection.Specials.Providers.Waiters;

namespace RedCatEngine.CommonServices.Services.Save.Applier
{
	public interface ISaveApplierContainer : IArrayWaiter<ISaved>
	{
		void Load(SaveData save);
		SaveData GenerateSaveWithName(string saveName);
	}
}