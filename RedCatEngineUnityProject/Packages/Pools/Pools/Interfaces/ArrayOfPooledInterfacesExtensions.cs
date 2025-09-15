namespace RedCatEngine.Pools.Pools.Interfaces
{
	public static class ArrayOfPooledInterfacesExtensions
	{
		public static void Enable(this IPooledEnable[] enables)
		{
			foreach (var enable in enables)
				enable.DoEnable();
		}

		public static void Disable(this IPooledDisable[] disables)
		{
			foreach (var disabled in disables)
				disabled.DoDisable();
		}

		public static void Reset(this IPooledReset[] resets)
		{
			foreach (var reset in resets)
				reset.Reset();
		}

		public static void DisableLogicBeforeTeleport(this IPooledTeleportedLogic[] teleportedArray)
		{
			foreach (var teleported in teleportedArray)
				teleported.DisableLogicBeforeTeleport();
		}

		public static void EnableLogicAfterTeleport(this IPooledTeleportedLogic[] teleportedArray)
		{
			foreach (var teleported in teleportedArray)
				teleported.EnableLogicAfterTeleport();
		}
	}
}