using RedCatEngine.DependencyInjection.Containers.Interfaces.Application;

namespace RedCatEngine.Values.Base.Interfaces
{
	public interface IValue<out TResultType>
	{
		TResultType GetValue(IGetterApplicationContainer getterContainer);
	}
}