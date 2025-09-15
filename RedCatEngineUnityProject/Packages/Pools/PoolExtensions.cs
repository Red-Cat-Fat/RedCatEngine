using RedCatEngine.Pools.Pools;

namespace RedCatEngine.Pools
{
	public static class PoolExtensions
	{
		public static void SetActive(this IPooledObject pooledObject, bool state)
		{
			if (state)
				pooledObject.Enable();
			else
				pooledObject.Disable();
		}
	}
}