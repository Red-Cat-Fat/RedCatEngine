namespace RedCatEngine.Pools.Pools.Interfaces
{
	public interface IPooledTeleportedLogic
	{
		void DisableLogicBeforeTeleport();
		void EnableLogicAfterTeleport();
	}
}