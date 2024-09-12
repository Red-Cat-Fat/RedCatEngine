using UnityEngine;

namespace RedCatEngine.DependencyInjection.Containers.Interfaces.Unity
{
	public interface IMonoConstructor
	{
		MonoBehaviour MonoConstruct(MonoBehaviour gameView, params object[] context);
		GameObject MonoConstruct(GameObject gameObject, bool constructChildren = false, params object[] context);
	}
}