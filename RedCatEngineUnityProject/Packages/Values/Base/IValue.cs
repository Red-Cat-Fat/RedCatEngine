using RedCatEngine.DependencyInjection.Containers.Interfaces;

namespace RedCatEngine.Values.Base
{
	public interface IValue<out TResultType>
	{
		TResultType GetValue(IApplicationContainer applicationContainer);
	}
}