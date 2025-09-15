namespace RedCatEngine.CommonServices.Services.Times.TimeServices
{
	public interface IPausedTimeService
	{
		void Pause(object source);
		void UnPause(object source);
	}
}