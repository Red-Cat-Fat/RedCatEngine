using RedCatEngine.DependencyInjection.Containers.Interfaces;

namespace RedCatEngine.Conditions.Base
{
	public interface ICondition
	{
		bool Check(IApplicationContainer applicationContainer);
	}
}