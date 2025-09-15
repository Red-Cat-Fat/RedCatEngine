using UnityEngine;

namespace RedCatEngine.CommonServices.Extensions
{
	public static class EditorExtensions
	{
		public static bool IsPrefab(this GameObject go)
			=> go.scene.rootCount == 0;
	}
}