using System.Collections.Generic;

namespace RedCatEngine.DependencyInjection.Containers.Interfaces
{
	public interface IGetterApplicationContainer
	{
		bool TryGetSingle<T>(out T data);
		bool TryGetArray<T>(out IEnumerable<T> data);
		T GetSingle<T>();
		IEnumerable<T> GetArray<T>();
	}
}