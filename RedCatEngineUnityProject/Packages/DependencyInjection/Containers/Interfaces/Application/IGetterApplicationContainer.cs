using System;
using System.Collections.Generic;

namespace RedCatEngine.DependencyInjection.Containers.Interfaces.Application
{
	public interface IGetterApplicationContainer
	{
		bool TryGetSingle<T>(out T data);
		bool TryGetSingle(Type type, out object data);
		bool TryGetArray<T>(out IEnumerable<T> data);
		T GetSingle<T>(params object[] context);
		object GetSingle(Type type, params object[] context);
		IEnumerable<T> GetArray<T>();
	}
}