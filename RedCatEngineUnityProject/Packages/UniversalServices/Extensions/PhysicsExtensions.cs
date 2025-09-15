using UnityEngine;

namespace RedCatEngine.CommonServices.Extensions
{
	public static class PhysicsExtensions
	{
		public static bool CompareLayer(this LayerMask layermask, int layer)
		{
			return layermask == (layermask | (1 << layer));
		}
	}
}