using System.Collections.Generic;

namespace RedCatEngine.DependencyInjection.Containers
{
	public interface IApplicationContainer
	{
		void BindAsSingle<T>(T instance);
		void BindAsArray<T>(T instance);
		bool TryGetSingle<T>(out T data);
		bool TryGetArray<T>(out IEnumerable<T> data);
		T GetSingle<T>();
		IEnumerable<T> GetArray<T>();
	}
}