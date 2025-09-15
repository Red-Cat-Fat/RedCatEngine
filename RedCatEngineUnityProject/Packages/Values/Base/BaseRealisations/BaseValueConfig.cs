using RedCatEngine.Configs;
using RedCatEngine.DependencyInjection.Containers.Interfaces.Application;
using RedCatEngine.Values.Base.Interfaces;
using SerializeReferenceEditor;

namespace RedCatEngine.Values.Base.BaseRealisations
{
	[SRHidden]
	public abstract class BaseValueConfig<TValue> : BaseConfig, IValue<TValue>
	{
		protected abstract IValue<TValue> ReturnValue { get; }

		public TValue GetValue(IGetterApplicationContainer getterContainer)
		{
			return ReturnValue.GetValue(getterContainer);
		}
	}
}