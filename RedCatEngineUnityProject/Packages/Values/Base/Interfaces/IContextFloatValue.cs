using RedCatEngine.DependencyInjection.Containers.Interfaces.Application;

namespace RedCatEngine.Values.Base.Interfaces
{
	public interface IContextFloatValue<in TContext> : IFloatValue
	{
		float GetValue(IGetterApplicationContainer getterContainer, TContext context);
	}
}