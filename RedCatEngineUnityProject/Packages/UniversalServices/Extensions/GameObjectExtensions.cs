using UnityEngine;

namespace RedCatEngine.CommonServices.Extensions
{
	public static class GameObjectExtensions
	{
		public static bool IsHasComponent<TBehaviour>(this MonoBehaviour otherComponent, out TBehaviour component)
			where TBehaviour : MonoBehaviour
		{
			component = null;
			return otherComponent != null && otherComponent.gameObject.TryGetComponent(out component);
		}

		public static bool IsHasComponent<TBehaviour>(this GameObject gameObject, out TBehaviour component)
			where TBehaviour : MonoBehaviour
		{
			component = null;
			return gameObject != null && gameObject.TryGetComponent(out component);
		}
	}
}