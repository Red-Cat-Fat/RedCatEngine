using System;

namespace RedCatEngine.DependencyInjection.Containers.Interfaces.Application.GenerationBind
{
	public interface ICreator
	{
		object Create(Type type, params object[] context);

		T Create<T>(params object[] context);
	}
}