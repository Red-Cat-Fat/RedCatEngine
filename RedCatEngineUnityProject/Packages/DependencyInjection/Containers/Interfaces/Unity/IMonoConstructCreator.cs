using RedCatEngine.DependencyInjection.Specials.Components;
using UnityEngine;

namespace RedCatEngine.DependencyInjection.Containers.Interfaces.Unity
{
	public interface IMonoConstructCreator
	{
		void MonoConstruct<TMonoBehaviour>(TMonoBehaviour gameView, params object[] context)
			where TMonoBehaviour : IMonoConstruct;

		GameObject MonoConstruct(
			GameObject gameObject,
			params object[] context
		);
	}
}