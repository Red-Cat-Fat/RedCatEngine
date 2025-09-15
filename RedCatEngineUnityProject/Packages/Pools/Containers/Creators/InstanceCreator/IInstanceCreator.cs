using RedCatEngine.Pools.Pools;

namespace RedCatEngine.Pools.Containers.Creators.InstanceCreator
{
	public interface IInstanceCreator
	{
		IPooledObject Create(params object[] context);
	}
}