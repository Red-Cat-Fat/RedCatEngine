using System;

namespace RedCatEngine.DependencyInjection.Containers.Interfaces
{
	public interface ICreator
	{
		object Create(Type type, params object[] context);

		T Create<T>(params object[] context);
	}
}