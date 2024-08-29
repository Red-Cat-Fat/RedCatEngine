using UnityEngine;

namespace RedCatEngine.DependencyInjection.Containers.Interfaces
{
	public interface IMonoConstructor
	{
		MonoBehaviour MonoConstruct(MonoBehaviour gameView, params object[] context);
	}
}