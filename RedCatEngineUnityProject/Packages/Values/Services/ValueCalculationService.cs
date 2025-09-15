using JetBrains.Annotations;
using RedCatEngine.DependencyInjection.Containers.Attributes;
using RedCatEngine.DependencyInjection.Containers.Interfaces.Application;
using RedCatEngine.Values.Base.Interfaces;

namespace RedCatEngine.Values.Services
{
	[UsedImplicitly]
	public class ValueCalculationService
	{
		private readonly IGetterApplicationContainer _getter;

		[Inject]
		public ValueCalculationService(IGetterApplicationContainer getter)
		{
			_getter = getter;
		}

		public float GetValueOrDefault(IFloatValue floatValue, float defaultValue)
			=> floatValue?.GetValue(_getter) ?? defaultValue;

		public virtual float GetValue(IFloatValue floatValue)
			=> floatValue.GetValue(_getter);

		public float GetValue<TContext>(IContextFloatValue<TContext> floatValue, TContext context)
			=> floatValue.GetValue(_getter, context);

		public bool GetValue(IBoolValue boolValue)
			=> boolValue.GetValue(_getter);

		public int GetValue(IIntValue intValue)
			=> intValue.GetValue(_getter);

		public ValueCalculationService Multiply(IFloatValue multiplier)
			=> new MultiplierValueCalculationService(_getter, multiplier);
	}
}