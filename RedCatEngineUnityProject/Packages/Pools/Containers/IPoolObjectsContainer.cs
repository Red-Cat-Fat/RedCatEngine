using UnityEngine;

namespace RedCatEngine.Pools.Containers
{
	public interface IPoolObjectsContainer : IPoolContainer
	{
		public GameObject Instantiate(params object[] context);
	}
}