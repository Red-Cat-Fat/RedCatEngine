using System.Collections.Generic;
using UnityEngine;

namespace RedCatEngine.Pools.Containers.Enumerators
{
	public class GameObjectsEnumerator : PooledObjectTypedEnumerator<GameObject>
	{
		public GameObjectsEnumerator(IEnumerable<GameObject> collection) : base(collection)
		{
		}
	}
}