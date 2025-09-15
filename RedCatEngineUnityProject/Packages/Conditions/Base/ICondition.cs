using RedCatEngine.DependencyInjection.Containers.Interfaces.Application;

namespace RedCatEngine.Conditions.Base
{
	public interface ICondition
	{
		bool Check(IGetterApplicationContainer getter);
	}
}