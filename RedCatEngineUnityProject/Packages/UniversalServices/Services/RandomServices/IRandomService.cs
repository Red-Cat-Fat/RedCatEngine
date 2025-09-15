namespace RedCatEngine.CommonServices.Services.RandomServices
{
	public interface IRandomService
	{
		void SetSeed(int seed);
		int GetRange(int minInclude, int maxExecute);
		float GetRange(float minInclude, float maxExecute);
		float ErrorRate(float baseValue, float error);
		float ErrorRate(float baseValue, float error, float min, float max);
	}
}