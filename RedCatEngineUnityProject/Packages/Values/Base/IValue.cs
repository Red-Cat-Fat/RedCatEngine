using RedCatEngine.DependencyInjection.Containers.Interfaces;
using RedCatEngine.DependencyInjection.Containers.Interfaces.Application;

namespace RedCatEngine.Values.Base
{
	public interface IValue<out TResultType>
	{
		TResultType GetValue(IApplicationContainer applicationContainer);
	}
}