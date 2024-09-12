using RedCatEngine.Configs;
using RedCatEngine.DependencyInjection.Containers.Interfaces;
using RedCatEngine.DependencyInjection.Containers.Interfaces.Application;

namespace RedCatEngine.Values.Base
{
	public abstract class BaseValueConfig<TValue> : BaseConfig, IValue<TValue>
	{
		protected abstract IValue<TValue> ReturnValue { get; }

		public TValue GetValue(IApplicationContainer applicationContainer)
		{
			return ReturnValue.GetValue(applicationContainer);
		}
	}
}